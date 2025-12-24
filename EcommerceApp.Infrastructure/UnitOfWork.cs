using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Infrastructure.Database;

namespace EcommerceApp.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public IProductRepository Products { get; set; }
        public IAuthRepository Auth { get; set; }
        private readonly ApplicationDbContext db;
        public UnitOfWork(ApplicationDbContext db, IProductRepository Products, IAuthRepository Auth)
        {
            this.Products = Products;
            this.Auth = Auth;
            this.db = db;
        }
        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
