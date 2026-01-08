using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Application.Interfaces.User;
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
        private readonly ICurrentUser user;
        public OrdersController(IOrderService orderService, ICurrentUser user)
        {
            this.user = user;
            this.orderService = orderService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders ()
        {
            // If user is Admin send all orders of web.
            if (user.IsInRole(AppRoles.Admin))
            {
                var allOrders = await orderService.GetAllOrders();
                return Ok(ApiResponse.SuccessResponse("Orders fetched successfully.", allOrders));
            }
            
            // If user is Customer send his orders only.
            if (user.IsInRole(AppRoles.Customer))
            {
                var customerOrders = await orderService.GetAllCustomerOrders();
                return Ok(ApiResponse.SuccessResponse("Orders fetched successfully.", customerOrders));
            }

            // If user is Seller send his orders only.
            if (user.IsInRole(AppRoles.Seller))
            {
                var customerOrders = await orderService.GetAllSellerOrdersOfSeller();
                return Ok(ApiResponse.SuccessResponse("Orders fetched successfully.", customerOrders));
            }

            return Forbid("Route Forbidden.");
        }

        //[Authorize(Roles = AppRoles.Customer)]
        //[HttpGet]
        //public async Task<IActionResult> GetCustomerOrders([FromQuery] OrderStatus? status)
        //{
        //    if (status is null)
        //    {
        //        var result = await orderService.GetAllCustomerOrders();
        //        if (result.Count() == 0) return NotFound(ApiResponse.ErrorResponse("No orders found.", null));
        //        return Ok(ApiResponse.SuccessResponse("Orders fetched successfully.", result));
        //    }

        //    var filteredResult = await orderService.GetCustomerOrdersByStatus((OrderStatus) status);
        //    if (filteredResult.Count() == 0) return NotFound(ApiResponse.ErrorResponse("No filtered orders found.", null));
        //    return Ok(ApiResponse.SuccessResponse("Filtered orders fetched successfully.", filteredResult));
        //}
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
        [HttpPost("Feedback")]
        public async Task<IActionResult> SubmitFeedback (FeedbackDto feedbackDto)
        {
            var result = await orderService.SubmitFeedback(feedbackDto);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("Feedback submission failed.", null));
            return Ok(ApiResponse.SuccessResponse("Feedback submitted.", null));
        }
    }
}
