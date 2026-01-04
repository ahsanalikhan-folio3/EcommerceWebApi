using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService orderService;
        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [Authorize(Roles = AppRoles.Customer)]
        [HttpPost]
        public async Task<IActionResult> CreateOrder (OrderDto order)
        {
            var result = await orderService.CreateOrderAsync(order);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("Order creation failed.", null));
            return Ok(ApiResponse.SuccessResponse("Order created.", null));
        }
    }
}
