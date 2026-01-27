using EcommerceApp.Application.Interfaces.ProductImages;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext db;
        public ProductImageRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<ProductImage?> AddProductImage(ProductImage productImage)
        {
            var result = await db.ProductImages.AddAsync(productImage);
            return result.Entity;
        }

        public async Task<List<ProductImage>?> AddProductImages(ICollection<ProductImage> productImages)
        {
            await db.ProductImages.AddRangeAsync(productImages);
            return productImages.ToList();
        }

        public async Task<bool> DeleteProductImageById(int id)
        {
            ProductImage productImage = (await this.GetProductImageById(id))!;
            db.Remove(productImage);
            return true;
        }

        public async Task<ProductImage?> GetProductImageById(int id)
        {
            return await db.ProductImages.FirstOrDefaultAsync(pi => pi.Id == id);
        }

        public async Task<int> GetProductImagesCountByProductId(int id)
        {
            return await db.ProductImages.CountAsync(pi => pi.ProductId == id);
        }
    }
}
