using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Dtos
{
    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }

    public class OrderDto
    {
        public required ICollection<OrderDto> OrderItems { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
