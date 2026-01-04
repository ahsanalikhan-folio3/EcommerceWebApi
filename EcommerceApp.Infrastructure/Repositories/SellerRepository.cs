using EcommerceApp.Application.Interfaces.Sellers;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class SellerRepository : ISellerRepository
    {
        private readonly ApplicationDbContext db;
        public SellerRepository(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<bool> AddSellerProfile(SellerProfile sellerProfile)
        {
            var result = await db.SellerProfiles.AddAsync(sellerProfile);
            return (result is not null) ? true : false;
        }

        public async Task<bool> SellerExistAsync(int UserId)
        {
            return await db.SellerProfiles.AnyAsync(c => c.UserId == UserId);
        }
    }
}
