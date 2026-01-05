using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly ICurrentUser user;
        public OrderService(IUnitOfWork uow, IMapper mapper, ICurrentUser user)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.user = user;
        }
        public async Task<bool> CreateOrderAsync(OrderDto order)
        {
            if (order == null || order.sellerOrders == null || !order.sellerOrders.Any())
                return false;
           
            /*
                Automatically maps SellerOrderDto -> SellerOrder and starts tracking it hence we
                dont have to explicitly track it and it will be automatically inserted in the
                SellerOrder table.
            */
            var mappedOrder = mapper.Map<Order>(order);
            mappedOrder.TotalAmount = 0;

            // Fetch all products in one call for efficiency
            var productIds = order.sellerOrders.Select(p => p.ProductId).ToList();
            var dbProducts = await uow.Products.GetProductByIds(productIds);
            var productDict = dbProducts.ToDictionary(p => p.Id);

            foreach (var item in mappedOrder.SellerOrders)
            {
                if (!productDict.TryGetValue(item.ProductId, out var dbProduct))
                    return false; // Invalid product

                if (!dbProduct.IsAvailable || dbProduct.StockQuantity < item.Quantity)
                    return false; // Product not available or insufficient stock

                item.Status = OrderStatus.Pending;

                // Update total amount
                mappedOrder.TotalAmount += dbProduct.Price * item.Quantity;

                // Decrease the stock of the product
                var product = productDict[item.ProductId];
                product.StockQuantity -= item.Quantity;
            }

            // Add order
            mappedOrder.OrderDate = DateTime.UtcNow;
            var createdOrder = await uow.Orders.CreateOrderAsync(mappedOrder);

            await uow.SaveChangesAsync();

            return true;
        }
        public async Task<bool> CancelOrder (CancelOrderDto cancelOrderDto)
        {
            var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(cancelOrderDto.SellerOrderId);
            var parentOrder = await uow.Orders.GetByIdAsync(sellerOrder!.OrderId);
            var product = await uow.Products.GetProductById(sellerOrder!.ProductId);

            if (sellerOrder == null || parentOrder == null || product == null)
                return false;

            // Cancel the seller order
            sellerOrder.Status = OrderStatus.Cancelled;
            
            // Update the parent order's total amount
            parentOrder.TotalAmount -= sellerOrder.Quantity * product.Price;
            
            // Restock the product
            product.StockQuantity += sellerOrder.Quantity;

            await uow.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SubmitFeedback(FeedbackDto feedbackDto)
        {
            var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(feedbackDto.SellerOrderId);
            if (sellerOrder == null) return false;

            var customer = await uow.Customers.GetCustomerById(user.GetUserIdInt());
            if (customer == null) return false;

            var product = await uow.Products.GetProductById(sellerOrder.ProductId);
            if (product == null) return false;

            var seller = await uow.Sellers.GetSellerById(product.SellerId);
            if (seller == null) return false;

            var feedback = mapper.Map<Feedback>(feedbackDto);
            feedback.GivenAt = DateTime.UtcNow;
            feedback.Customer = customer; 
            feedback.CorrespondingSellerOrder = sellerOrder;
            feedback.Seller = seller;

            await uow.Feedbacks.AddFeedbackAsync(feedback);
            await uow.SaveChangesAsync(); // saving first to ensure it's in DB

            // Recomputing seller rating safely in DB
            seller.Rating = await uow.Feedbacks.GetAverageRatingOfAllFeedbacksOfSeller(seller.UserId);

            await uow.SaveChangesAsync();

            return true;
        }

    }
}
