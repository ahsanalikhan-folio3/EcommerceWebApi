using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Customers
{
    public interface ICustomerRepository
    {
        Task<bool> CustomerExistAsync(int UserId);
        Task<bool> AddCustomerProfile(CustomerProfile customerProfile);
        Task<CustomerProfile?> GetCustomerById(int id);
    }
}
