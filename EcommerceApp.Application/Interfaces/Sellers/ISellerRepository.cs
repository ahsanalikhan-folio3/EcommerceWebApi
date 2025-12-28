using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Sellers
{
    public interface ISellerRepository
    {
        Task<bool> SellerExistAsync(Guid UserId);
        Task<bool> AddSellerProfile(SellerProfile sellerProfile);
    }
}
