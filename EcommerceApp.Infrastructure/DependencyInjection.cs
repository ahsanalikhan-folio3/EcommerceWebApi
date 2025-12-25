using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.AdminProfiles;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Infrastructure.Database;
using EcommerceApp.Infrastructure.Repositories;
using EcommerceApp.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceApp.Infrastructure
{
    static public class DependencyInjection
    {
        static public IServiceCollection AddInfrastructure (this IServiceCollection services, string dbConnectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(dbConnectionString);
            });
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAdminProfileRepository, AdminProfileRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
