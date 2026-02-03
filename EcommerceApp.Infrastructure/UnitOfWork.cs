using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Admins;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.CancelledOrders;
using EcommerceApp.Application.Interfaces.Chats;
using EcommerceApp.Application.Interfaces.Customers;
using EcommerceApp.Application.Interfaces.Feedbacks;
using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Application.Interfaces.ProductImages;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.Interfaces.Sellers;
using EcommerceApp.Infrastructure.Database;

namespace EcommerceApp.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public IProductRepository Products { get; set; }
        public IProductImageRepository ProductImages { get; set; }
        public IAuthRepository Auth { get; set; }
        public ICustomerRepository Customers { get; set; }
        public ISellerRepository Sellers { get; set; }
        public IAdminRepository Admins  { get; set; }
        public ISellerOrderRepository SellerOrders  { get; set; }
        public ICancelledOrderRepository CancelledOrders  { get; set; }
        public IOrderRepository Orders  { get; set; }
        public IFeedbackRepository Feedbacks { get; set; }
        public IChatRepository Chats { get; set; }
        public IMessageRepository Messages { get; set; }

        private readonly ApplicationDbContext db;
        public UnitOfWork(ApplicationDbContext db, IProductRepository products,
            IProductImageRepository productsImages, IAuthRepository auth,
            ICustomerRepository customers, ISellerRepository sellers,
            IAdminRepository admins, ISellerOrderRepository sellerOrders,
            IOrderRepository orders, IFeedbackRepository feedbacks,
            IChatRepository chats, IMessageRepository messages, ICancelledOrderRepository cancelledOrders)
        {
            this.db = db;
            Products = products;
            ProductImages = productsImages;
            Auth = auth;
            Customers = customers;
            Sellers = sellers;
            Admins = admins;
            SellerOrders = sellerOrders;
            Orders = orders;
            Feedbacks = feedbacks;
            Chats = chats;
            Messages = messages;
            CancelledOrders = cancelledOrders;
        }
        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
