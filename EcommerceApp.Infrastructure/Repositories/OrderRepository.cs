using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext db;
        public OrderRepository(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<Order> CreateOrderAsync(Order order)
        {
            var result = await db.Orders.AddAsync(order);
            return result.Entity;
        }
        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await db.Orders.FindAsync(orderId);
        }
        public async Task<List<Order>> GetAllOrdersOfUserByIdAsync(int userId)
        {
            return await db.Orders.Include(o => o.SellerOrders).ThenInclude(s => s.OrderedProduct).Where(o => o.UserId == userId).ToListAsync();
        }
    }
}
