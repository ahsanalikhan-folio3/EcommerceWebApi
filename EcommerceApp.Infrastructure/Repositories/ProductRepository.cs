using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext db;

        public ProductRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<Product> AddProduct(Product product)
        {
            var result = await db.Products.AddAsync(product);
            return result.Entity;
        }

        public async Task<bool> DeleteProductById(int Id)
        {
            Product product = (await this.GetProductById(Id))!;
            db.Remove(product);
            return true;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var result = await db.Products.Include(pi => pi.ProductImages).ToListAsync();
            return result;
        }

        public async Task<List<Product>> GetAllSellerProducts(int sellerId)
        {
            return await db.Products.AsNoTracking().Where(x => x.SellerId == sellerId).ToListAsync();
        }

        public async Task<Product?> GetProductById(int Id)
        {
            var result = await db.Products.FindAsync(Id);
            return result;
        }

        public async Task<ICollection<Product>> GetProductByIds(ICollection<int> Ids)
        {
            return await db.Products.Where(x => Ids.Contains(x.Id)).ToListAsync();
        }
        public async Task<bool> ProductBelongsToSellerAsync(int productId, int sellerId)
        {
            return await db.Products.AnyAsync(x => x.Id == productId && x.SellerId == sellerId);
        }
        public async Task<bool> ProductExistsAsync(string ProductSlug)
        {
            return await db.Products.AnyAsync(x => x.ProductSlug == ProductSlug);
        }

        public Product UpdateProduct(Product product)
        {
            var result = db.Update(product);
            return result.Entity;
        }
    }
}
