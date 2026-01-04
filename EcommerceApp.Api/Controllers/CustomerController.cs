using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Customers;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;
        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }
        //[HttpPost("{id}")]
        //public async Task<IActionResult> CreateCustomerProfile(int id, CustomerProfileDto customerProfileDto)
        //{
        //    var result = await customerService.AddCustomerProfile(id, customerProfileDto);
        //    if (!result) return BadRequest(ApiResponse.ErrorResponse("Customer Profile already exists", null));
        //    return Ok(ApiResponse.SuccessResponse("Customer Profile successfully created", null));
        //}
    }
}
