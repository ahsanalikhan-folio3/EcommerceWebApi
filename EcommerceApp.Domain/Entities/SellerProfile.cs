namespace EcommerceApp.Domain.Entities
{
    public class SellerProfile
    {
        public Guid UserId { get; set; } // FK - ApplicationUsers Table
        public required string Storename { get; set; }
        public required string StoreAddress { get; set; }
        public decimal Rating { get; set; }
    }
}
