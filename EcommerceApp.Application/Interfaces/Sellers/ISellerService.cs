using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Sellers
{
    public interface ISellerService
    {
        Task<List<GetProductDto>> GetAllSellerProducts();
        Task<GetProductDto?> AddProduct(ProductDto Product);
        Task<GetProductDto?> UpdateProduct(int productId, ProductDto product);
        Task<bool> UpdateOrderStatus(UpdateSellerOrderStatusFromSellerSideDto UpdateSellerOrderStatusFromSellerSideDto);
    }
}
