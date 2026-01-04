using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Orders
{
    public interface ISellerOrderRepository
    {
        Task<bool> SellerOrderExistAsync(int SellerOrderId);
        Task<int?> GetProductId(int sellerOrderId);
        Task AddSellerOrders(ICollection<SellerOrder> SellerOrders);
        Task<IEnumerable<SellerOrder>> GetSellerOrdersByOrderId(int orderId);
        Task<SellerOrder?> GetSellerOrdersById(int SellerOrderId);
        Task<bool> UpdateSellerOrderStatus(int SellerOrderId, OrderStatus status);
    }
}
