namespace EcommerceApp.Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public required string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Product CorrespondingProduct { get; set; } // Navigation Property
    }
}
