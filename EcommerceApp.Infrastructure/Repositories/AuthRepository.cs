using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext db;
        public AuthRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<EmailVerificationOtp> AddEmailVerificationOtp (EmailVerificationOtp emailVerificationOtp)
        {
            var result = await db.EmailVerificationOtps.AddAsync(emailVerificationOtp);
            return result.Entity;
        }
        public async Task DeleteAllEmailVerificationOtp(int userId)
        {
            await db.EmailVerificationOtps
                .Where(x => x.UserId == userId)
                .ExecuteDeleteAsync();
        }

        public async Task<ApplicationUser> AddUser(ApplicationUser user)
        {
            var result = await db.ApplicationUsers.AddAsync(user);
            return result.Entity;
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await db.ApplicationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);        
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(int id)
        {
            return await db.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<AdminProfile?> GetAdminProfileByIdAsync(int id)
        {
            return await db.AdminProfiles
                .Include(u => u.User)
                .FirstOrDefaultAsync(x => x.UserId == id);
        }
        public async Task<SellerProfile?> GetSellerProfileByIdAsync(int id)
        {
            return await db.SellerProfiles
                .Include(u => u.User)
                .FirstOrDefaultAsync(x => x.UserId == id);
        }
        public async Task<CustomerProfile?> GetCustomerProfileByIdAsync(int id)
        {
            return await db.CustomerProfiles
                .Include(u => u.User)
                .FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<string?> GetUserRoleAsync(string email)
        {
            return await db.ApplicationUsers
                .AsNoTracking()
                .Where(x => x.Email == email)
                .Select(x => x.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UserActiveAsync(string email)
        {
            return await db.ApplicationUsers.AnyAsync(x => x.Email == email && x.IsActive);
        }

        public async Task<bool> UserExistByEmailAsync(string email)
        {
            return await db.ApplicationUsers.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> UserExistByIdAsync(int Id)
        {
            return await db.ApplicationUsers.AnyAsync(x => x.Id == Id);
        }

        public async Task<EmailVerificationOtp?> GetEmailVerificationOtp(int userId)
        {
            return await db.EmailVerificationOtps
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();
        }
    }
}
