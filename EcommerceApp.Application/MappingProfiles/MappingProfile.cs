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
            CreateMap<ApplicationUser, RegisterUserDto>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();

            // AdminProfile User Mappings
            CreateMap<AdminProfile, AdminProfileDto>().ReverseMap();
            CreateMap<AdminProfileDto, RegisterUserDto>().ReverseMap();
            CreateMap<GetAdminProfileDto, AdminProfileDto>().ReverseMap();

            // CustomerProfile User Mappings
            CreateMap<CustomerProfile, CustomerProfileDto>().ReverseMap();
            CreateMap<CustomerProfileDto, RegisterUserDto>().ReverseMap();
            CreateMap<GetCustomerProfileDto, CustomerProfileDto>().ReverseMap();

            // SellerProfile User Mappings
            CreateMap<SellerProfile, SellerProfileDto>().ReverseMap();
            CreateMap<SellerProfileDto, RegisterUserDto>().ReverseMap();
            CreateMap<GetSellerProfileDto, SellerProfileDto>().ReverseMap();

            // SellerOrder Mappings
            CreateMap<SellerOrder, SellerOrderDto>().ReverseMap();
            CreateMap<SellerOrder, GetSellerOrderDto>().ReverseMap();

            // Order Mappings
            CreateMap<Order, OrderDto>().ReverseMap();

            // Feedback Mappings
            CreateMap<Feedback, FeedbackDto>().ReverseMap();
            CreateMap<Feedback, GetFeedbackDto>().ReverseMap();
        }
    }
}
