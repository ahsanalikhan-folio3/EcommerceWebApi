using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Customers
{
    public interface ICustomerService
    {
        Task<bool> AddCustomerProfile(Guid Id, CustomerProfileDto customerProfileDto);
    }
}
