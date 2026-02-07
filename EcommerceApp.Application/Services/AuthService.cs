using AutoMapper;
using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Exceptions;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.JobServices;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

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
        private readonly IConfiguration configuration;
        public AuthService(IUnitOfWork uow, IMapper mapper, IJwtService jwtService, IUserService user, IBackgroundJobService backgroundJobService, IConfiguration configuration)
        {
            this.user = user;
            this.uow = uow;
            this.mapper = mapper;
            this.jwtService = jwtService;
            this.backgroundJobService = backgroundJobService;
            this.passwordHasher = new PasswordHasher<ApplicationUser>();
            this.configuration = configuration;
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

            await this.SendEmailVerificationOtp(appUser.Email);

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
            appUser.IsActive = true; // Directly register admins
            appUser.IsEmailVerified = true; // Directly register admins
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

            await this.SendEmailVerificationOtp(appUser.Email);

            GetSellerProfileDto mappedResult = mapper.Map<GetSellerProfileDto>(seller);
            mappedResult.Id = appUser.Id;
            return mappedResult;
        }
        
        public async Task<bool> SendEmailVerificationOtp (string email) 
        {
            // 1️- Generate a secure 6-digit OTP
            var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            // 2️- Hash the OTP 
            using var sha256 = SHA256.Create();
            var otpHashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(otp));
            var otpHash = Convert.ToBase64String(otpHashBytes);

            var appUser = await uow.Auth.GetUserByEmailAsync(email);
            if (appUser == null) return false;

            // Delete any existing OTP for the user before adding a new one
            await uow.Auth.DeleteAllEmailVerificationOtp(appUser.Id);

            var emailVerificationOtp = new EmailVerificationOtp()
            {
                UserId = appUser.Id,
                HashedOtp = otpHash,
                OtpAttempts = 0,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10), // Fetch this from environment variable later
            };

            await uow.Auth.AddEmailVerificationOtp(emailVerificationOtp);
            await uow.SaveChangesAsync();

            // Send Email
            backgroundJobService.EnqueueOtpEmailJob(appUser.Email, otp, 10);
            return true;
        }
        public async Task<bool> VerifyEmailVerificationOtp(EmailVerificationDto dto)
        {
            var currTime = DateTime.UtcNow;

            var user = await uow.Auth.GetUserByEmailAsync(dto.Email);
            if (user == null) return false;

            var emailOtp = await uow.Auth.GetEmailVerificationOtp(user.Id);
            if (emailOtp == null) return false;

            // 1️- Expiry check
            if (currTime > emailOtp.ExpiryTime)
            {
                await uow.Auth.DeleteAllEmailVerificationOtp(user.Id);
                await uow.SaveChangesAsync();
                return false;
            }

            // 2️- Hash incoming OTP
            using var sha256 = SHA256.Create();
            var otpHash = Convert.ToBase64String(
                sha256.ComputeHash(Encoding.UTF8.GetBytes(dto.Otp)));

            // 3️- Compare
            if (otpHash == emailOtp.HashedOtp)
            {
                user.IsEmailVerified = true;
                await uow.Auth.DeleteAllEmailVerificationOtp(user.Id);
                await uow.SaveChangesAsync();

                // Enqueue background job to send different email based on the role of the user.
                if (user.Role == AppRoles.Seller) backgroundJobService.EnqueueAccountReviewOfSellerOnRegistrationEmailJob(user.Email);

                if (user.Role == AppRoles.Customer) backgroundJobService.EnqueueCustomerWelcomeEmailJob(user.Email);

                return true;
            }

            // 4️- Failed attempt
            emailOtp.OtpAttempts++;

            if (emailOtp.OtpAttempts >= 3)
                await uow.Auth.DeleteAllEmailVerificationOtp(user.Id);

            await uow.SaveChangesAsync();
            return false;
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
            } else throw new BusinessRuleException("User role is not valid.");
        }
    }
}
