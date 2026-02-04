using EcommerceApp.Application.Dtos;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Orders
{
    public interface IOrderService
    {
        Task<bool> SellerOrderBelongsToCustomer(int sellerOrderId);
        Task<bool> IsSellerOrderStatusDeliveredAsync(int sellerOrderId);
        Task<bool> SellerOrderBelongsToSeller(int sellerOrderId);
        Task<bool> SellerOrderExistAsync(int sellerOrderId);
        Task<List<OrderDetailsEmailDto>?> CreateOrderAsync(OrderDto orderItems);
        Task<bool> SubmitFeedback(int sellerOrderId, FeedbackDto feedbackDto);
        Task<List<GetSellerOrderDto>> GetAllOrders();
        Task<IEnumerable<GetSellerOrderDto>> GetAllCustomerOrders();
        Task<IEnumerable<GetSellerOrderDto>> GetAllSellerOrdersOfSeller();
        Task<IEnumerable<GetSellerOrderDto>> GetCustomerOrdersByStatus(OrderStatus status);
        Task<bool> UpdateSellerOrderStatusForAdmin(int sellerOrderId, UpdateSellerOrderStatusDto updateSellerOrderStatusDto);
        Task<bool> UpdateSellerOrderStatusForSeller(int sellerOrderId, UpdateSellerOrderStatusDto updateSellerOrderStatusDto);
        Task<bool> UpdateSellerOrderStatusForCustomer(int sellerOrderId, UpdateSellerOrderStatusDto updateSellerOrderStatusDto);
    }
}
