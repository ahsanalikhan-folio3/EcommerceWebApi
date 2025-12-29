using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.Sellers;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class SellerService : ISellerService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        public SellerService(IUnitOfWork uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }
        //public async Task<bool> AddSellerProfile(Guid Id, SellerProfileDto SellerProfileDto)
        //{
        //    bool userAlreadyExist = await uow.Auth.UserExistByIdAsync(Id);
        //    bool SellerProfileAlreadyExist = await uow.Sellers.SellerExistAsync(Id);

        //    // If user does not exist or Seller profile already exists, return false
        //    if (!userAlreadyExist || SellerProfileAlreadyExist) return false;
        //    var mappedSellerProfile = mapper.Map<SellerProfile>(SellerProfileDto);

        //    mappedSellerProfile.UserId = Id;
        //    await uow.Sellers.AddSellerProfile(mappedSellerProfile);
        //    await uow.SaveChangesAsync();

        //    return true;
        //}
    }
}
