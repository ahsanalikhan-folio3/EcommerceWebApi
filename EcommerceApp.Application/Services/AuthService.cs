using AutoMapper;
using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EcommerceApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IJwtService jwtService;
        private readonly PasswordHasher<ApplicationUser> passwordHasher;
        public AuthService(IUnitOfWork uow, IMapper mapper, IJwtService jwtService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.jwtService = jwtService;
            this.passwordHasher = new PasswordHasher<ApplicationUser>();
        }
        public async Task<bool> UserExistAsyncById (int id)
        {
            return await uow.Auth.UserExistByIdAsync(id);
        }
        public async Task<bool> UserExistAsyncByEmail (string email)
        {
            return await uow.Auth.UserExistByEmailAsync(email);
        }
        public async Task<bool> UserIsActiveAsync (string email)
        {
            var user = await uow.Auth.GetUserByEmailAsync(email);
            return  user!.IsActive;
        }
        public async Task<bool> ValidatePassword (LoginDto user)
        {
            ApplicationUser appUser = (await uow.Auth.GetUserByEmailAsync(user.Email))!;
            var comparisionResult = passwordHasher.VerifyHashedPassword(appUser, appUser.HashedPassword, user.Password);
            return (comparisionResult == PasswordVerificationResult.Success);
        }
        public async Task<GetLoginResultDto> LoginUser(LoginDto user)
        {
            ApplicationUser appUser = (await uow.Auth.GetUserByEmailAsync(user.Email))!;

            string token = jwtService.GenerateToken(appUser);

            var result = new GetLoginResultDto() { Role = appUser.Role, Token = token};

            return result;
        }
        public async Task<bool> ChangeUserActivationStatus (int id, UserActivationDto userActivationDto)
        {
            var user = await uow.Auth.GetUserByIdAsync(id);
            user!.IsActive = userActivationDto.IsActive;
            await uow.SaveChangesAsync();

            return true;
        }
        private async Task<ApplicationUser?> RegisterUser(RegisterUserDto user)
        {
            // If user exist return false ( can not add the user ).
            if (await uow.Auth.UserExistByEmailAsync(user.Email)) return null;

            ApplicationUser mappedUser = mapper.Map<ApplicationUser>(user);
            mappedUser.HashedPassword = passwordHasher.HashPassword(mappedUser, user.Password); // Hash the password.
            mappedUser.CreatedAt = DateTime.UtcNow;
            mappedUser.IsActive = false; // For normal user this will get true when he/she verifies his/her email.
            
            var result = await uow.Auth.AddUser(mappedUser);
            if (result is not ApplicationUser) return null;

            return result;
        }
        public async Task<GetCustomerProfileDto?> RegisterCustomer(CustomerProfileDto customer)
        {
            RegisterUserDto appUserTableEntry = mapper.Map<RegisterUserDto>(customer);
            appUserTableEntry.Role = AppRoles.Customer;

            ApplicationUser? appUser = await RegisterUser(appUserTableEntry);
            if (appUser is null) return null;

            CustomerProfile customerProfile = mapper.Map<CustomerProfile>(customer);
            customerProfile.User = appUser;
            appUser.IsActive = true; // Directly register customers
            var result = await uow.Customers.AddCustomerProfile(customerProfile);
            if (!result) return null;

            await uow.SaveChangesAsync();

            GetCustomerProfileDto mappedResult = mapper.Map<GetCustomerProfileDto>(customer);
            return mappedResult;
        }
        public async Task<GetAdminProfileDto?> RegisterAdmin(AdminProfileDto admin)
        {
            RegisterUserDto appUserTableEntry = mapper.Map<RegisterUserDto>(admin);
            appUserTableEntry.Role = AppRoles.Admin;

            ApplicationUser? appUser = await RegisterUser(appUserTableEntry);
            if (appUser is null) return null;

            AdminProfile adminProfile = mapper.Map<AdminProfile>(admin);
            adminProfile.User = appUser;
            var result = await uow.Admins.AddAdminProfile(adminProfile);
            if (!result) return null;

            await uow.SaveChangesAsync();

            GetAdminProfileDto mappedResult = mapper.Map<GetAdminProfileDto>(admin);
            return mappedResult;
        }

        public async Task<GetSellerProfileDto?> RegisterSeller(SellerProfileDto seller)
        {
            RegisterUserDto appUserTableEntry = mapper.Map<RegisterUserDto>(seller);
            appUserTableEntry.Role = AppRoles.Seller;

            ApplicationUser? appUser = await RegisterUser(appUserTableEntry);
            if (appUser is null) return null;

            SellerProfile sellerProfile = mapper.Map<SellerProfile>(seller);
            sellerProfile.User = appUser;
            var result = await uow.Sellers.AddSellerProfile(sellerProfile);
            if (!result) return null;

            await uow.SaveChangesAsync();

            GetSellerProfileDto mappedResult = mapper.Map<GetSellerProfileDto>(seller);
            return mappedResult;
        }
    }
}
