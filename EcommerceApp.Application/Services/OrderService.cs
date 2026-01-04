using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        public OrderService(IUnitOfWork uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
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
                //var product = productDict[item.ProductId];
                //product.StockQuantity -= item.Quantity;
            }

            // Add order
            mappedOrder.OrderDate = DateTime.UtcNow;
            var createdOrder = await uow.Orders.CreateOrderAsync(mappedOrder);

            await uow.SaveChangesAsync();

            return true;
        }
    }
}
