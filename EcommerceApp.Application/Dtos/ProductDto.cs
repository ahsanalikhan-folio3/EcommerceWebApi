using Microsoft.AspNetCore.Http;

namespace EcommerceApp.Application.Dtos
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public required string ImageUrl { get; set; }
    }
    public class ProductDto
    {
        public required string Name { get; set; }
        public required string ProductSlug { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required int StockQuantity { get; set; }
        public required decimal Price { get; set; }
        public required bool IsAvailable { get; set; }
    }
    public class GetProductDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ProductSlug { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required int StockQuantity { get; set; }
        public required decimal Price { get; set; }
        public required bool IsAvailable { get; set; }
        public required ICollection<ProductImageDto> ProductImages { get; set; }
    }
    public class ProductImageUploadDto
    {
        public ICollection<IFormFile> ImageFiles { get; set; } = null!;
    }
}
