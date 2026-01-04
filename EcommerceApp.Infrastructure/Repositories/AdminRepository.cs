using EcommerceApp.Application.Interfaces.Admins;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext db;
        public AdminRepository(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<bool> ActivateUserAsync(int userId)
        {
            var user = await db.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            user.IsActive = true;

            await db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddAdminProfile(AdminProfile adminProfile)
        {
            var result = await db.AdminProfiles.AddAsync(adminProfile);
            return result is not null;
        }
        public async Task<bool> AdminExistAsync(int UserId)
        {
            return await db.AdminProfiles.AnyAsync(x => x.UserId == UserId);
        }

        public async Task<bool> DeActivateUserAsync(int userId)
        {
            var user = await db.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            user.IsActive = false;

            await db.SaveChangesAsync();
            return true;
        }
    }
}
