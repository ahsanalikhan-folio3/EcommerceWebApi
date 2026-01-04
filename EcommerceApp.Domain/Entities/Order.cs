namespace EcommerceApp.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public required ICollection<SellerOrder> SellerOrders { get; set; }
    }
}
