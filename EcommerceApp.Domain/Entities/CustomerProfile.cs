namespace EcommerceApp.Domain.Entities
{
    public class CustomerProfile
    {
        public Guid UserId { get; set; } // FK - ApplicationUsers Table
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string? Gender { get; set; }
    }
}
