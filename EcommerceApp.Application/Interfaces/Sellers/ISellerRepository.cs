using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Sellers
{
    public interface ISellerRepository
    {
        Task<bool> SellerExistAsync(int UserId);
        Task<bool> AddSellerProfile(SellerProfile sellerProfile);
        Task<SellerProfile?> GetSellerById(int id);
    }
}
