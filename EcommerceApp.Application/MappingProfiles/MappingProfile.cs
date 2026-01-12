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
            CreateMap<AdminProfile, GetAdminProfileDto>()
                 .ForMember(dest => dest.Email,
                     opt => opt.MapFrom(src => src.User.Email))
                 .ForMember(dest => dest.PhoneNumber,
                     opt => opt.MapFrom(src => src.User.PhoneNumber))
                 .ForMember(dest => dest.FullName,
                     opt => opt.MapFrom(src => src.User.FullName))
                 .ForMember(dest => dest.Role,
                     opt => opt.MapFrom(src => src.User.Role))
                 .ReverseMap();

            // CustomerProfile User Mappings
            CreateMap<CustomerProfile, CustomerProfileDto>().ReverseMap();
            CreateMap<CustomerProfileDto, RegisterUserDto>().ReverseMap();
            CreateMap<GetCustomerProfileDto, CustomerProfileDto>().ReverseMap();
            CreateMap<CustomerProfile, GetCustomerProfileDto>()
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.User.Role))
                .ReverseMap();


            // SellerProfile User Mappings
            CreateMap<SellerProfile, SellerProfileDto>().ReverseMap();
            CreateMap<SellerProfileDto, RegisterUserDto>().ReverseMap();
            CreateMap<GetSellerProfileDto, SellerProfileDto>().ReverseMap();
            CreateMap<SellerProfile, GetSellerProfileDto>()
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.User.Role))
                .ReverseMap();

            // SellerOrder Mappings
            CreateMap<SellerOrder, SellerOrderDto>().ReverseMap();
            CreateMap<SellerOrder, GetSellerOrderDto>().ReverseMap();

            // Order Mappings
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Order, GetOrderDto>().ReverseMap();

            // Feedback Mappings
            CreateMap<Feedback, FeedbackDto>().ReverseMap();
            CreateMap<Feedback, GetFeedbackDto>().ReverseMap();
        }
    }
}
