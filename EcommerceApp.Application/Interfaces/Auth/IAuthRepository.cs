using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Auth
{
    public interface IAuthRepository
    {
        public Task<bool> UserExistAsync(string email);
        public Task<bool> UserActiveAsync(string email);
        public Task<string?> GetUserRoleAsync(string email);
        public Task<ApplicationUser?> GetUserByEmailAsync(string email);
        public Task<ApplicationUser> AddUser(ApplicationUser user);
    }
}
