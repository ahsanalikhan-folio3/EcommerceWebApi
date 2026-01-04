using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Admins
{
    public interface IAdminService
    {
        //Task<bool> AddAdminProfile(AdminProfileDto adminProfile);
        Task<bool> ActivateUser(int userId);
        Task<bool> DeActivateUser(int userId);
        Task<bool> UpdateSellerOrderStatus(UpdateSellerOrderStatusFromAdminSideDto updateSellerOrderStatusFromAdminSide);
    }
}
