using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product Mappings
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, GetProductDto>().ReverseMap();

            // Application User Mappings
            CreateMap<ApplicationUser, RegisterDto>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();

            // AdminProfile User Mappings
            CreateMap<AdminProfile, AdminProfileDto>().ReverseMap();

            // CustomerProfile User Mappings
            CreateMap<CustomerProfile, CustomerProfileDto>().ReverseMap();
        }
    }
}
