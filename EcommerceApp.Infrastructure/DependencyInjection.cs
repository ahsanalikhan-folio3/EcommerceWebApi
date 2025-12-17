using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Infrastructure.Database;
using EcommerceApp.Infrastructure.Repositories;
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
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
