using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Dtos
{
    public class SellerOrderDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderDto
    {
        public required ICollection<SellerOrderDto> sellerOrders { get; set; }
        public int UserId { get; set; }
    }
    public class CancelOrderDto
    {
        public int SellerOrderId { get; set; }
    }
}
