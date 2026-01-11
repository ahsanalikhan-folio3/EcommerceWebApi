using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.MappingProfiles;
using EcommerceApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.Admins;
using EcommerceApp.Application.Interfaces.Customers;
using EcommerceApp.Application.Interfaces.Sellers;
using EcommerceApp.Application.Interfaces.Orders;

namespace EcommerceApp.Application
{
    static public class DependencyInjection
    {
        static public IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
