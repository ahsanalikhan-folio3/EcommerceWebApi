using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Customers
{
    public interface ICustomerRepository
    {
        Task<bool> CustomerExistAsync(Guid UserId);
        Task<bool> AddCustomerProfile(CustomerProfile customerProfile);
    }
}
