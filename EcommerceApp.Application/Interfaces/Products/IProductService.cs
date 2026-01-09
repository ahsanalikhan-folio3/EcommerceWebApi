using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Products
{
    public interface IProductService
    {
        Task<List<GetProductDto>> GetAllProducts();
        Task<List<GetFeedbackDto>?> GetProductFeedbacks(int productId);
        Task<List<GetSellerOrderDto>?> GetProductOrders(int productId);
        Task<List<GetProductDto>> GetAllSellerProducts();
        Task<GetProductDto?> GetProductById(int Id);
        Task<GetProductDto?> AddProduct(ProductDto Product);
        Task<bool> ProductBelongsToSellerAsync(int productId);
        Task<bool> ProductExistsAsync(int productId);
        Task<GetProductDto?> UpdateProduct(int Id, ProductDto Product);
        Task<bool> DeleteProduct(int Id);
    }
}
