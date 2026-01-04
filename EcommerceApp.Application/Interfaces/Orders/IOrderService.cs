using EcommerceApp.Application.Dtos;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Orders
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(OrderDto orderItems);
    }
}
