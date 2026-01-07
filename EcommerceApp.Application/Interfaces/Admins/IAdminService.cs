using EcommerceApp.Application.Dtos;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Admins
{
    public interface IAdminService
    {
        //Task<bool> AddAdminProfile(AdminProfileDto adminProfile);
        Task<bool> ActivateUser(int userId);
        Task<bool> DeActivateUser(int userId);
        Task<bool> UpdateSellerOrderStatus(UpdateSellerOrderStatusFromAdminSideDto updateSellerOrderStatusFromAdminSide);
        Task<List<GetSellerOrderDto>> GetAllSellerOrders();
        Task<List<GetSellerOrderDto>> GetAllSellerOrdersByStatus(OrderStatus status);
    }
}
