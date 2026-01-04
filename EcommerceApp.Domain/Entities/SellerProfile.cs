namespace EcommerceApp.Domain.Entities
{
    public class SellerProfile
    {
        public int UserId { get; set; } // FK - ApplicationUsers Table
        public required ApplicationUser User { get; set; } // FK - ApplicationUsers Table
        public required string Storename { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
        public decimal Rating { get; set; }
    }
}
