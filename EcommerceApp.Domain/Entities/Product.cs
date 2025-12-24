namespace EcommerceApp.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string ProductSlug { get; set; }
        public required string Category { get; set; }
        public required int StockQuantity { get; set; }
        public required decimal Price { get; set; }
        public required bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<OrderItem> BelongedOrderItems { get; set; }
    }
}
