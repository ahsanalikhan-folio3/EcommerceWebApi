using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.ProductImages
{
    public interface IProductImageRepository
    {
        Task<List<ProductImage>?> AddProductImages(ICollection<ProductImage> productImages);
        Task<ProductImage?> AddProductImage(ProductImage productImage);
        Task<int> GetProductImagesCountByProductId(int id);
        Task<ProductImage?> GetProductImageById(int id);
        Task<bool> DeleteProductImageById(int id);
    }
}
