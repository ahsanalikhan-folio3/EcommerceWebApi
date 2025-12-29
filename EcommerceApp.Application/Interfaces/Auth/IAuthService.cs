using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        //public Task<ApplicationUserDto?> RegisterUser(RegisterUserDto user);
        public Task<GetAdminProfileDto?> RegisterAdmin(AdminProfileDto customer);
        public Task<GetCustomerProfileDto?> RegisterCustomer(CustomerProfileDto customer);
        public Task<GetSellerProfileDto?> RegisterSeller(SellerProfileDto customer);
        public Task<string?> LoginUser(LoginDto user);
    }
}
