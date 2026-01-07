using EcommerceApp.Application.Dtos;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Orders
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetByIdAsync(int orderId);
        Task<List<Order>> GetAllOrdersOfUserByIdAsync(int userId);
    }
}
