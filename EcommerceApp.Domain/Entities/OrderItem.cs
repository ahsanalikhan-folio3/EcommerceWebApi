namespace EcommerceApp.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public required Order CorresponingOrder { get; set; }
        public Guid ProductId { get; set; }
        public required Product OrderedProduct { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}
