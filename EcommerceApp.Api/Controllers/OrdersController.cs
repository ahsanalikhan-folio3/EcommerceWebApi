using EcommerceApp.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpPost]
        public Task<IActionResult> CreateOrder (OrderDto order)
        {
            throw new NotImplementedException();
        }
    }
}
