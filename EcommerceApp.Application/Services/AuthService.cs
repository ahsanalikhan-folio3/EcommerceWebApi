using AutoMapper;
using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.JobServices;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EcommerceApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IJwtService jwtService;
        private readonly IUserService user;
        private readonly IBackgroundJobService backgroundJobService;
        private readonly PasswordHasher<ApplicationUser> passwordHasher;
        public AuthService(IUnitOfWork uow, IMapper mapper, IJwtService jwtService, IUserService user, IBackgroundJobService backgroundJobService)
        {
            this.user = user;
            this.uow = uow;
            this.mapper = mapper;
            this.jwtService = jwtService;
            this.backgroundJobService = backgroundJobService;
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

            if (userActivationDto.IsActive) backgroundJobService.EnqueueAccountActivationEmailJob(user.Email);
            else backgroundJobService.EnqueueAccountDeactivationEmailJob(user.Email); 

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
            backgroundJobService.EnqueueCustomerWelcomeEmailJob(customer.Email);

            GetCustomerProfileDto mappedResult = mapper.Map<GetCustomerProfileDto>(customer);
            mappedResult.Id = appUser.Id;
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
            mappedResult.Id = appUser.Id;
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
            backgroundJobService.EnqueueAccountReviewOfSellerOnRegistrationEmailJob(seller.Email);

            GetSellerProfileDto mappedResult = mapper.Map<GetSellerProfileDto>(seller);
            mappedResult.Id = appUser.Id;
            return mappedResult;
        }

        public async Task<object?> GetUserProfileAsync()
        {
            int userId = user.GetUserIdInt();
            if (user.IsInRole(AppRoles.Admin))
            {
                var adminProfile = await uow.Auth.GetAdminProfileByIdAsync(userId);
                return mapper.Map<GetAdminProfileDto>(adminProfile);
            }
            else if (user.IsInRole(AppRoles.Customer))
            {
                var customerProfile = await uow.Auth.GetCustomerProfileByIdAsync(userId);
                return mapper.Map<GetCustomerProfileDto>(customerProfile);
            }
            else if (user.IsInRole(AppRoles.Seller))
            {
                var sellerProfile = await uow.Auth.GetSellerProfileByIdAsync(userId);
                return mapper.Map<GetSellerProfileDto>(sellerProfile);
            }
            return null;
        }
    }
}
