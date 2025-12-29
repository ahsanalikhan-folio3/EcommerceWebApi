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

    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public required ICollection<OrderItem> OrderItems { get; set; }
    }
}
