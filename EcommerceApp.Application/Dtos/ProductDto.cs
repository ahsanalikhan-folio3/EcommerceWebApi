namespace EcommerceApp.Application.Dtos
{
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
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string ProductSlug { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required int StockQuantity { get; set; }
        public required decimal Price { get; set; }
        public required bool IsAvailable { get; set; }
    }
}
