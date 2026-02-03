using EcommerceApp.Application.Interfaces.CancelledOrders;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class CancelledOrderRepository : ICancelledOrderRepository
    {
        private readonly ApplicationDbContext db;

        public CancelledOrderRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task AddCancelledOrderRecordAsync(CancelledOrder cancelledOrder)
        {
            await db.CancelledOrders.AddAsync(cancelledOrder);
        }
    }
}
