using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Interfaces.Customers;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        public CustomerService(IUnitOfWork uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }
        //public async Task<bool> AddCustomerProfile(Guid Id, CustomerProfileDto customerProfileDto)
        //{
        //    bool userAlreadyExist = await uow.Auth.UserExistByIdAsync(Id);
        //    bool customerProfileAlreadyExist = await uow.Customers.CustomerExistAsync(Id);

        //    // If user does not exist or customer profile already exists, return false
        //    if (!userAlreadyExist || customerProfileAlreadyExist) return false;
        //    var mappedCustomerProfile = mapper.Map<CustomerProfile>(customerProfileDto);

        //    mappedCustomerProfile.UserId = Id;
        //    await uow.Customers.AddCustomerProfile(mappedCustomerProfile);
        //    await uow.SaveChangesAsync();

        //    return true;
        //}
    }
}
