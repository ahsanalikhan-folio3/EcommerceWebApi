namespace EcommerceApp.Application.Dtos
{
    public class SellerProfileDto
    {
        public required string Storename { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }
}
