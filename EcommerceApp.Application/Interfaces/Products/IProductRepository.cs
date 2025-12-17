using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Products
{
    public interface IProductRepository
    {
        public Task<bool> ProductExistsAsync(string ProductSlug);
        public Task<List<Product>> GetAllProducts();
        public Task<Product?> GetProductById(Guid Id);
        public Task<Product> AddProduct(Product product);
        public Task<bool> DeleteProductById(Guid Id);
        public Product UpdateProduct(Product product);
    }
}
