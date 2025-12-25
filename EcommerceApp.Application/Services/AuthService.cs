using AutoMapper;
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
        public async Task<string?> LoginUser(LoginDto user)
        {
            // User must exist.
            if (!(await uow.Auth.UserExistAsync(user.Email))) return "Does not exist.";
            // User must be active.
            if (!(await uow.Auth.UserActiveAsync(user.Email))) return "InActive.";

            ApplicationUser appUser = (await uow.Auth.GetUserByEmailAsync(user.Email))!;

            // Comparing password.
            var comparisionResult = passwordHasher.VerifyHashedPassword(appUser, appUser.HashedPassword, user.Password);
            if (comparisionResult != PasswordVerificationResult.Success) return "Invalid Password.";

            string jwtToken = jwtService.GenerateToken(appUser);

            return $"You are a {appUser.Role} on my Ecommerce Website and your JwtToken is {jwtToken}";
        }

        public async Task<ApplicationUserDto?> RegisterUser(RegisterDto user)
        {
            // If user exist return false ( can not add the user ).
            if (await uow.Auth.UserExistAsync(user.Email)) return null;

            ApplicationUser mappedUser = mapper.Map<ApplicationUser>(user);
            mappedUser.HashedPassword = passwordHasher.HashPassword(mappedUser, user.Password); // Hash the password.
            mappedUser.CreatedAt = DateTime.UtcNow;
            mappedUser.IsActive = false; // For normal user this will get true when he/she verifies his/her email.
            
            var result = await uow.Auth.AddUser(mappedUser);
            if (result is not ApplicationUser) return null;
            await uow.SaveChangesAsync();

            return mapper.Map<ApplicationUserDto>(result);
        }
    }
}
