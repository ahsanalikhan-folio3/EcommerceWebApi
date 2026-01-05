namespace EcommerceApp.Domain.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public int UserId { get; set; } // FK - ApplicationUser
        public int SellerId { get; set; } // FK - SellerProfile
        public int SellerOrderId { get; set; } // FK - SellerOrder
        public required CustomerProfile Customer { get; set; }
        public required SellerProfile Seller { get; set; }
        public required SellerOrder CorrespondingSellerOrder { get; set; }
        public required string Comment { get; set; }
        public required int Rating { get; set; }
        public required DateTime GivenAt { get; set; }
    }
}
