using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.AdminProfiles
{
    public interface IAdminProfileRepository
    {
        Task<bool> AddAdminProfile(AdminProfile adminProfile);
        Task<bool> AdminExistAsync(Guid UserId);
    }
}
