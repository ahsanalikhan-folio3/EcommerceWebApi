using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Products
{
    public interface IProductRepository
    {
        Task<bool> ProductExistsAsync(string ProductSlug);
        Task<bool> ProductBelongsToSellerAsync(int productId, int sellerId);
        Task<List<Product>> GetAllProducts();
        Task<List<Product>> GetAllSellerProducts(int sellerId);
        Task<Product?> GetProductById(int Id);
        Task<ICollection<Product>> GetProductByIds(ICollection<int> Ids);
        Task<Product> AddProduct(Product product);
        Task<bool> DeleteProductById(int Id);
        Product UpdateProduct(Product product);
    }
}
