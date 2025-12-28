using EcommerceApp.Application.Interfaces.Admins;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.Customers;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.Interfaces.Sellers;

namespace EcommerceApp.Application.Interfaces
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; set; }
        public IAuthRepository Auth { get; set; }
        public IAdminRepository Admins { get; set; }
        public ICustomerRepository Customers { get; set; }
        public ISellerRepository Sellers { get; set; }
        public Task SaveChangesAsync();
    }
}
