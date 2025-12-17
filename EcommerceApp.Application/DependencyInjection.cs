using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.MappingProfiles;
using EcommerceApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;

namespace EcommerceApp.Application
{
    static public class DependencyInjection
    {
        static public IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
