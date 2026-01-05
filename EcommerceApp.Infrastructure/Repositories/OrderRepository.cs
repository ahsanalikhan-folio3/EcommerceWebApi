using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;

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
    }
}
