using EcommerceApp.Application.Interfaces.Customers;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext db;
        public CustomerRepository(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<bool> AddCustomerProfile(CustomerProfile customerProfile)
        {
            var result = await db.CustomerProfiles.AddAsync(customerProfile);
            return (result is not null) ? true : false;
        }

        public async Task<bool> CustomerExistAsync(int UserId)
        {
            return await db.CustomerProfiles.AnyAsync(c => c.UserId == UserId);
        }
    }
}
