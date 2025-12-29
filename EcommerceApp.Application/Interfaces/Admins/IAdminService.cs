using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Admins
{
    public interface IAdminService
    {
        //Task<bool> AddAdminProfile(AdminProfileDto adminProfile);
        Task<bool> ActivateUser(Guid userId);
        Task<bool> DeActivateUser(Guid userId);
    }
}
