using EcommerceApp.Application.Interfaces.AdminProfiles;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class AdminProfileRepository : IAdminProfileRepository
    {
        private readonly ApplicationDbContext db;
        public AdminProfileRepository(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<bool> AddAdminProfile(AdminProfile adminProfile)
        {
            var result = await db.AdminProfiles.AddAsync(adminProfile);
            return result is not null;
        }
        public async Task<bool> AdminExistAsync(Guid UserId)
        {
            return await db.AdminProfiles.AnyAsync(x => x.UserId == UserId);
        }
    }
}
