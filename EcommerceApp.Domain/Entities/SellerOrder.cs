namespace EcommerceApp.Domain.Entities
{
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        InWarehouse = 3,
        Shipped = 4,
        Delivered = 5,
        Cancelled = 6
    }
    public class SellerOrder
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public required Order CorresponingOrder { get; set; }
        public required Product OrderedProduct { get; set; }
        public OrderStatus Status { get; set; }
    }
}
