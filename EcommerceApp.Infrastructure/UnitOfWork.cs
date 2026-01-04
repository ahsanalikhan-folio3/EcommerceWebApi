using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Admins;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.Customers;
using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.Interfaces.Sellers;
using EcommerceApp.Infrastructure.Database;

namespace EcommerceApp.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public IProductRepository Products { get; set; }
        public IAuthRepository Auth { get; set; }
        public ICustomerRepository Customers { get; set; }
        public ISellerRepository Sellers { get; set; }
        public IAdminRepository Admins  { get; set; }
        public ISellerOrderRepository SellerOrders  { get; set; }
        public IOrderRepository Orders  { get; set; }
        private readonly ApplicationDbContext db;
        public UnitOfWork(ApplicationDbContext db, IProductRepository Products, IAuthRepository Auth, 
            ICustomerRepository Customers, ISellerRepository Sellers, 
            IAdminRepository Admins, ISellerOrderRepository SellerOrders,
            IOrderRepository Orders)
        {
            this.Products = Products;
            this.Auth = Auth;
            this.Customers = Customers;
            this.Sellers = Sellers;
            this.Admins = Admins;
            this.db = db;
            this.SellerOrders = SellerOrders;
            this.Orders = Orders;
        }
        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
