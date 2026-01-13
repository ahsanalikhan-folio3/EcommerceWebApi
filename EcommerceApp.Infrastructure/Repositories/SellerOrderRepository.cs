using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class SellerOrderRepository : ISellerOrderRepository
    {
        private readonly ApplicationDbContext db;
        public SellerOrderRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        // Check if order item exists by id
        public async Task<bool> SellerOrderExistAsync (int SellerOrderId)
        {
            return await db.SellerOrders.AnyAsync(x => x.Id == SellerOrderId);
        }

        // Add multiple order items to the database
        public async Task AddSellerOrders(ICollection<SellerOrder> SellerOrders)
        {
            await db.SellerOrders.AddRangeAsync(SellerOrders);
        }

        // Get order item by id
        public Task<SellerOrder?> GetSellerOrdersById(int SellerOrderId)
        {
            return db.SellerOrders
                .FirstOrDefaultAsync(oi => oi.Id == SellerOrderId);
        }
        // Get all order items by order id
        public async Task<IEnumerable<SellerOrder>> GetSellerOrdersByOrderId(int orderId)
        {
            return await db.SellerOrders
                .AsNoTracking()
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }
        // Update order item status
        public async Task<bool> UpdateSellerOrderStatus(int SellerOrderId, OrderStatus status)
        {
            SellerOrder? order = await db.SellerOrders.FirstOrDefaultAsync(x => x.Id == SellerOrderId);
            if (order is null)
                return false;
            order.Status = status;
            return true;
        }
        // Get product id by seller order id
        public async Task<int?> GetProductId(int sellerOrderId)
        {
            var sellerOrder = await db.SellerOrders.FirstOrDefaultAsync(x => x.Id == sellerOrderId);
            return sellerOrder?.ProductId;
        }
        // Get all seller orders of a product
        public async Task<List<SellerOrder>> GetAllSellerOrdersOfProduct (int productId)
        {
            return await db.SellerOrders.Where(x => x.ProductId == productId).ToListAsync();
        }
        // Get all seller orders of a product along with product details
        public async Task<List<SellerOrder>> GetAllSellerOrdersOfProductAlongWithProduct(int productId)
        {
            return await db.SellerOrders.Include(e => e.OrderedProduct).Where(e => e.ProductId == productId).ToListAsync();
        }
        // Get all seller orders along with product details
        public async Task<List<SellerOrder>> GetAllSellerOrders ()
        {
            return await db.SellerOrders.Include(e => e.OrderedProduct).ToListAsync();
        }
        // Get all seller orders of a seller along with product details
        public async Task<List<SellerOrder>> GetAllSellerOrdersOfSeller (int sellerId)
        {
            return await db.SellerOrders.Include(p => p.OrderedProduct).Where(p => p.OrderedProduct.SellerId == sellerId).ToListAsync();
        }
        // Get all seller orders of a customer along with order and product details
        public async Task<List<SellerOrder>> GetAllSellerOrdersOfCustomer (int userId)
        {
            return await db.SellerOrders.Include(p => p.CorresponingOrder).Include(p => p.OrderedProduct).Where(p => p.CorresponingOrder.UserId == userId).ToListAsync();
        }
        // Get seller orders by order id along with product details
        public async Task<List<SellerOrder>> GetSellerOrderByOrderIdAlongWithProductDetails (int orderId)
        {
            return await db.SellerOrders.Include(p => p.OrderedProduct).Where(p => p.OrderId == orderId).ToListAsync();
        }
    }
}
