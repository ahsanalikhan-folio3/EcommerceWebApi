using EcommerceApp.Application.Common;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Admins;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.Chats;
using EcommerceApp.Application.Interfaces.Customers;
using EcommerceApp.Application.Interfaces.Feedbacks;
using EcommerceApp.Application.Interfaces.Jobs;
using EcommerceApp.Application.Interfaces.JobServices;
using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.Interfaces.Sellers;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Infrastructure.Database;
using EcommerceApp.Infrastructure.Jobs;
using EcommerceApp.Infrastructure.JobService;
using EcommerceApp.Infrastructure.Repositories;
using EcommerceApp.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceApp.Infrastructure
{
    static public class DependencyInjection
    {
        static public IServiceCollection AddInfrastructure (this IServiceCollection services, string dbConnectionString, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(dbConnectionString);
            });
            services.AddHttpContextAccessor();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISellerRepository, SellerRepository>(); 
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ISellerOrderRepository, SellerOrderRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();

            // Search what is perfect for a background job.
            services.AddTransient<IEmailJob, EmailJob>();
            services.AddTransient<IBackgroundJobService, HangfireBackgroundJobService>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            return services;
        }
    }
}
