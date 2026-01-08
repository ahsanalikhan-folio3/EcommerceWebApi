using EcommerceApp.Application.Dtos;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Orders
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(OrderDto orderItems);
        Task<bool> CancelOrder(CancelOrderDto cancelOrderDto);
        Task<bool> SubmitFeedback(FeedbackDto feedbackDto);
        Task<List<GetSellerOrderDto>> GetAllOrders();
        Task<IEnumerable<GetSellerOrderDto>> GetAllCustomerOrders();
        Task<IEnumerable<GetSellerOrderDto>> GetAllSellerOrdersOfSeller();
        Task<IEnumerable<GetSellerOrderDto>> GetCustomerOrdersByStatus(OrderStatus status);
    }
}
