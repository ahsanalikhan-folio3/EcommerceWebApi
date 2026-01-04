using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Admins
{
    public interface IAdminRepository
    {
        Task<bool> AddAdminProfile(AdminProfile adminProfile);
        Task<bool> AdminExistAsync(int UserId);
        Task<bool> ActivateUserAsync(int UserId);
        Task<bool> DeActivateUserAsync(int UserId);
    }
}
