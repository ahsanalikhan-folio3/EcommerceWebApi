namespace EcommerceApp.Domain.Entities
{
    public class CancelledOrder
    {
        public int Id { get; set; }
        public int SellerOrderId { get; set; }
        public int CancelledById { get; set; }
        public required string Role { get; set; }
        public string? Reason { get; set; }
        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;
        public SellerOrder CorrespondingSellerOrder { get; set; }
        public ApplicationUser CancelledByUser { get; set; }
    }
}
