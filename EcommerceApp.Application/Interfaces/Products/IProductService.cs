using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Products
{
    public interface IProductService
    {
        public Task<List<GetProductDto>> GetAllProducts();
        public Task<GetProductDto?> GetProductById(int Id);
        //public Task<GetProductDto?> AddProduct(ProductDto Product);
        public Task<GetProductDto?> UpdateProduct(int Id, ProductDto Product);
        public Task<bool> DeleteProduct(int Id);
    }
}
