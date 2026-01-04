using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Dtos
{
    //public class AdminProfileDto
    //{
    //    public int UserId { get; set; }
    //}
    public class UpdateSellerOrderStatusFromAdminSideDto
    {
        public int SellerOrderId { get; set; }
        public OrderStatus Status { get; set; }
    }
}
