using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Products
{
    public interface IProductService
    {
        public Task<List<GetProductDto>> GetAllProducts();
        public Task<GetProductDto?> GetProductById(Guid Id);
        public Task<GetProductDto?> AddProduct(ProductDto Product);
        public Task<GetProductDto?> UpdateProduct(Guid Id, ProductDto Product);
        public Task<bool> DeleteProduct(Guid Id);
    }
}
