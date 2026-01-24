using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.JobServices;
using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IUserService user;
        private readonly IBackgroundJobService backgroundJobService;
        public OrderService(IUnitOfWork uow, IMapper mapper, IUserService user, IBackgroundJobService backgroundJobService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.user = user;
            this.backgroundJobService = backgroundJobService;
        }
        public async Task<bool> SellerOrderExistAsync (int sellerOrderId)
        {
            var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(sellerOrderId);
            return sellerOrder is not null;
        }
        public async Task<bool> CreateOrderAsync(OrderDto order)
        {
            if (order == null || order.SellerOrders == null || !order.SellerOrders.Any())
                return false;
           
            /*
                Automatically maps SellerOrderDto -> SellerOrder and starts tracking it hence we
                dont have to explicitly track it and it will be automatically inserted in the
                SellerOrder table.
            */
            var mappedOrder = mapper.Map<Order>(order);
            mappedOrder.TotalAmount = 0;

            // Fetch all products in one call for efficiency
            var productIds = order.SellerOrders.Select(p => p.ProductId).ToList();
            var dbProducts = await uow.Products.GetProductByIds(productIds);
            var productDict = dbProducts.ToDictionary(p => p.Id);

            List<OrderDetailsEmailDto> orderDetailsEmailDtos = new List<OrderDetailsEmailDto>();
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

            // Prepare email details
            decimal totalAmount = mappedOrder.TotalAmount;
            var sellerOrdersAlongWithProductDetails = await uow.SellerOrders.GetSellerOrderByOrderIdAlongWithProductDetails(createdOrder.Id);
            for (int i = 0; i < sellerOrdersAlongWithProductDetails.Count(); i++)
            {
                var item = sellerOrdersAlongWithProductDetails.ElementAt(i);
                orderDetailsEmailDtos.Add(new OrderDetailsEmailDto { SellerOrderId = item.Id, ProductName = item.OrderedProduct.Name, Quantity = item.Quantity });
            }

            // Send Email in background
            backgroundJobService.EnqueueSuccessfullOrderCompletionEmailJob(user.Email!, totalAmount, orderDetailsEmailDtos);

            return true;
        }
        public async Task<bool> SubmitFeedback(int sellerOrderId, FeedbackDto feedbackDto)
        {
            var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(sellerOrderId);
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
        public async Task<List<GetSellerOrderDto>> GetAllOrders()
        {
            var allOrders = await uow.SellerOrders.GetAllSellerOrders();
            return mapper.Map<List<GetSellerOrderDto>>(allOrders);
        }
        public async Task<IEnumerable<GetSellerOrderDto>> GetAllSellerOrdersOfSeller ()
        {
            int sellerId = user.GetUserIdInt();
            var allSellerOrders = await uow.SellerOrders.GetAllSellerOrdersOfSeller(sellerId);
            return mapper.Map<IEnumerable<GetSellerOrderDto>>(allSellerOrders);
        }
        public async Task<IEnumerable<GetSellerOrderDto>> GetAllCustomerOrders()
        {
            int customerId = user.GetUserIdInt();
            var allCustomerOrders = await uow.SellerOrders.GetAllSellerOrdersOfCustomer(customerId);
            return mapper.Map<IEnumerable<GetSellerOrderDto>>(allCustomerOrders);
        }
        public async Task<IEnumerable<GetSellerOrderDto>> GetCustomerOrdersByStatus(OrderStatus status)
        {
            var allOrders = await GetAllCustomerOrders();
            return allOrders.Where(o => o.Status == status);
        }
        public async Task<bool> UpdateSellerOrderStatusForAdmin(int sellerOrderId, UpdateSellerOrderStatusDto updateSellerOrderStatusDto)
        {
            await uow.SellerOrders.UpdateSellerOrderStatus(sellerOrderId, updateSellerOrderStatusDto.Status);
            await uow.SaveChangesAsync();

            var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(sellerOrderId);
            var parentOrder = await uow.Orders.GetByIdAsync(sellerOrder!.OrderId);
            var customer = await uow.Auth.GetUserByIdAsync(parentOrder!.UserId);
            backgroundJobService.EnqueueOrderStatusUpdateEmailJob(customer!.Email!, sellerOrderId, updateSellerOrderStatusDto.Status);

            return true;
        }
        public async Task<bool> UpdateSellerOrderStatusForSeller(int sellerOrderId, UpdateSellerOrderStatusDto updateSellerOrderStatusDto)
        {
            var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(sellerOrderId);
            OrderStatus status = updateSellerOrderStatusDto.Status;

            // If order is in pending state, seller can only update it to either processing or cancelled.
            if (sellerOrder!.Status == OrderStatus.Pending)
            {
                if (status == OrderStatus.Processing || status == OrderStatus.Cancelled)
                {
                    await uow.SellerOrders.UpdateSellerOrderStatus(sellerOrderId, status);
                    await uow.SaveChangesAsync();

                    var parentOrder = await uow.Orders.GetByIdAsync(sellerOrder.OrderId);
                    var customer = await uow.Auth.GetUserByIdAsync(parentOrder!.UserId);
                    backgroundJobService.EnqueueOrderStatusUpdateEmailJob(customer!.Email, sellerOrderId, updateSellerOrderStatusDto.Status);

                    return true;
                }
            }

            return false;
        }
        public async Task<bool> UpdateSellerOrderStatusForCustomer(int sellerOrderId, UpdateSellerOrderStatusDto updateSellerOrderStatusDto)
        {
            var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(sellerOrderId);

            // Customer can only cancel pending orders
            if (sellerOrder!.Status != OrderStatus.Pending || updateSellerOrderStatusDto.Status != OrderStatus.Cancelled)    
                return false;

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

            var sellerId = product.SellerId;
            var seller = await uow.Auth.GetUserByIdAsync(sellerId);
            backgroundJobService.EnqueueOrderStatusUpdateEmailJob(seller!.Email, sellerOrderId, updateSellerOrderStatusDto.Status);
            
            return true;
        }

        public async Task<bool> SellerOrderBelongsToCustomer(int sellerOrderId)
        {
            var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(sellerOrderId);
            if (sellerOrder == null) return false;
            var order = await uow.Orders.GetByIdAsync(sellerOrder.OrderId);
            return order is null || order.UserId == user.GetUserIdInt();
        }

        public async Task<bool> SellerOrderBelongsToSeller(int sellerOrderId)
        {
            var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(sellerOrderId);
            if (sellerOrder == null) return false;
            var product = await uow.Products.GetProductById(sellerOrder.ProductId);
            return product is null || product.SellerId == user.GetUserIdInt();
        }

        public async Task<bool> IsSellerOrderStatusDeliveredAsync(int sellerOrderId)
        {
            var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(sellerOrderId); 
            return sellerOrder != null && sellerOrder.Status == OrderStatus.Delivered;
        }
    }
}
