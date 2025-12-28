using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Sellers
{
    public interface ISellerService
    {
        Task<bool> AddSellerProfile(Guid Id, SellerProfileDto sellerProfileDto);
    }
}
