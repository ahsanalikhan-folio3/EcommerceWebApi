using EcommerceApp.Application.Dtos;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Sellers
{
    public interface ISellerService
    {
        Task<List<GetProductDto>> GetAllSellerProducts();
        Task<GetProductDto?> AddProduct(ProductDto Product);
        Task<GetProductDto?> UpdateProduct(int productId, ProductDto product);
        Task<bool> UpdateOrderStatus(UpdateSellerOrderStatusFromSellerSideDto UpdateSellerOrderStatusFromSellerSideDto);
        Task<List<GetFeedbackDto>> GetProductFeedbacks(int productId);
        Task<List<GetSellerOrderDto>> GetProductOrders(int productId);
        Task<bool> ProductBelongsToUserAsync(int productId);
        Task<List<GetSellerOrderDto>> GetProductOrdersByStatus(int productId, OrderStatus status);
    }
}
