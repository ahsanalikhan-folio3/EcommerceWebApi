using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Auth
{
    public interface IAuthRepository
    {
        Task<bool> UserExistByEmailAsync(string email);
        Task<bool> UserExistByIdAsync(int Id);
        Task<bool> UserActiveAsync(string email);
        Task<string?> GetUserRoleAsync(string email);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserByIdAsync(int id);
        Task<AdminProfile?> GetAdminProfileByIdAsync(int id);
        Task<SellerProfile?> GetSellerProfileByIdAsync(int id);
        Task<CustomerProfile?> GetCustomerProfileByIdAsync(int id);
        Task<ApplicationUser> AddUser(ApplicationUser user);
    }
}
