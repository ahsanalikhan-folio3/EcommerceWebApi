using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        //public Task<ApplicationUserDto?> RegisterUser(RegisterUserDto user);
        Task<GetAdminProfileDto?> RegisterAdmin(AdminProfileDto customer);
        Task<GetCustomerProfileDto?> RegisterCustomer(CustomerProfileDto customer);
        Task<GetSellerProfileDto?> RegisterSeller(SellerProfileDto customer);
        Task<bool> UserExistAsyncByEmail(string email);
        Task<bool> UserExistAsyncById(int id);
        Task<bool> UserIsActiveAsync(string email);
        Task<bool> ValidatePassword(LoginDto user);
        Task<GetLoginResultDto> LoginUser(LoginDto user);
        Task<bool> ChangeUserActivationStatus(int id, UserActivationDto userActivationDto);
        Task<object?> GetUserProfileAsync();
    }
}
