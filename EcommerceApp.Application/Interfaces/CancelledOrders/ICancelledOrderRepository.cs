using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.CancelledOrders
{
    public interface ICancelledOrderRepository
    {
        Task AddCancelledOrderRecordAsync(CancelledOrder cancelledOrder);
    }
}
