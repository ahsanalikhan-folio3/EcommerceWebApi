namespace EcommerceApp.Application.Dtos
{
    public class OrderDetailsEmailDto
    {
        public int SellerOrderId { get; set; }
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
