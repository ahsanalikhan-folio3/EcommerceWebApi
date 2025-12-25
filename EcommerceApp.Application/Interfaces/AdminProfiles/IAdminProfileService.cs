using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.AdminProfiles
{
    public interface IAdminProfileService
    {
        Task<bool> AddAdminProfile(AdminProfileDto adminProfile);
    }
}
