using EcommerceApp.Application.Interfaces.Admins;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.Chats;
using EcommerceApp.Application.Interfaces.Customers;
using EcommerceApp.Application.Interfaces.Feedbacks;
using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Application.Interfaces.ProductImages;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.Interfaces.Sellers;

namespace EcommerceApp.Application.Interfaces
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; set; }
        public IProductImageRepository ProductImages { get; set; }
        public IAuthRepository Auth { get; set; }
        public IAdminRepository Admins { get; set; }
        public ICustomerRepository Customers { get; set; }
        public ISellerRepository Sellers { get; set; }
        public ISellerOrderRepository SellerOrders { get; set; }
        public IOrderRepository Orders { get; set; }
        public IFeedbackRepository Feedbacks { get; set; }
        public IChatRepository Chats { get; set; }
        public IMessageRepository Messages { get; set; }
        public Task SaveChangesAsync();
    }
}
