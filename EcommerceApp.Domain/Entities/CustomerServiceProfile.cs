namespace EcommerceApp.Domain.Entities
{
    public class CustomerServiceProfile
    {
        public int UserId { get; set; } // FK - ApplicationUsers Table
        public required ApplicationUser User { get; set; } // FK - ApplicationUsers Table
        public required SellerProfile SellerUser { get; set; } // FK - SellerProfile Table
    }
}
