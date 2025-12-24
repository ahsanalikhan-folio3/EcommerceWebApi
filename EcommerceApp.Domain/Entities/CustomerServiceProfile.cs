namespace EcommerceApp.Domain.Entities
{
    public class CustomerServiceProfile
    {
        public Guid UserId { get; set; } // (FK - ApplicationUsers Table)
        public Guid SellerId { get; set; } // Seller that created this User
    }
}
