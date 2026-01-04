using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Auth
{
    public interface IAuthRepository
    {
        public Task<bool> UserExistByEmailAsync(string email);
        public Task<bool> UserExistByIdAsync(int Id);
        public Task<bool> UserActiveAsync(string email);
        public Task<string?> GetUserRoleAsync(string email);
        public Task<ApplicationUser?> GetUserByEmailAsync(string email);
        public Task<ApplicationUser> AddUser(ApplicationUser user);
    }
}
