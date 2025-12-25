using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        public Task<ApplicationUserDto?> RegisterUser(RegisterDto user);
        public Task<string?> LoginUser(LoginDto user);
    }
}
