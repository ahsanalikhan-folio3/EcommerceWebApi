using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Admins
{
    public interface IAdminRepository
    {
        Task<bool> AddAdminProfile(AdminProfile adminProfile);
        Task<bool> AdminExistAsync(Guid UserId);
        Task<bool> ActivateUserAsync(Guid UserId);
        Task<bool> DeActivateUserAsync(Guid UserId);
    }
}
