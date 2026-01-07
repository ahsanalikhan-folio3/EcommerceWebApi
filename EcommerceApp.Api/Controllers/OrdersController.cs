using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Domain.Entities;
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
        [HttpGet]
        public async Task<IActionResult> GetCustomerOrders([FromQuery] OrderStatus? status)
        {
            if (status is null)
            {
                var result = await orderService.GetCustomerOrders();
                if (result.Count() == 0) return NotFound(ApiResponse.ErrorResponse("No orders found.", null));
                return Ok(ApiResponse.SuccessResponse("Orders fetched successfully.", result));
            }

            var filteredResult = await orderService.GetCustomerOrdersByStatus((OrderStatus) status);
            if (filteredResult.Count() == 0) return NotFound(ApiResponse.ErrorResponse("No filtered orders found.", null));
            return Ok(ApiResponse.SuccessResponse("Filtered orders fetched successfully.", filteredResult));
        }
        [Authorize(Roles = AppRoles.Customer)]
        [HttpPost]
        public async Task<IActionResult> CreateOrder (OrderDto order)
        {
            var result = await orderService.CreateOrderAsync(order);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("Order creation failed.", null));
            return Ok(ApiResponse.SuccessResponse("Order created.", null));
        }

        [Authorize(Roles = AppRoles.Customer)]
        [HttpPut("Cancel")]
        public async Task<IActionResult> CancelOrder (CancelOrderDto cancelOrderDto)
        {
            var result = await orderService.CancelOrder(cancelOrderDto);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("Order cancellation failed.", null));
            return Ok(ApiResponse.SuccessResponse("Order cancelled.", null));
        }

        [Authorize(Roles = AppRoles.Customer)]
        [HttpPut("Feedback")]
        public async Task<IActionResult> SubmitFeedback (FeedbackDto feedbackDto)
        {
            var result = await orderService.SubmitFeedback(feedbackDto);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("Feedback submission failed.", null));
            return Ok(ApiResponse.SuccessResponse("Feedback submitted.", null));
        }
    }
}
