using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        //public Task<ApplicationUserDto?> RegisterUser(RegisterUserDto user);
        Task<GetAdminProfileDto?> RegisterAdmin(AdminProfileDto customer);
        Task<GetCustomerProfileDto?> RegisterCustomer(CustomerProfileDto customer);
        Task<GetSellerProfileDto?> RegisterSeller(SellerProfileDto customer);
        Task<bool> UserExistAsync(string email);
        Task<bool> UserIsActiveAsync(string email);
        Task<bool> ValidatePassword(LoginDto user);
        Task<GetLoginResultDto> LoginUser(LoginDto user);
    }
}
