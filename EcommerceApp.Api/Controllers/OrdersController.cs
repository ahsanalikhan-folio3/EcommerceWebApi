using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Orders;
using EcommerceApp.Application.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly IUserService user;
        public OrdersController(IOrderService orderService, IUserService user)
        {
            this.user = user;
            this.orderService = orderService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
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
                var sellerOrders = await orderService.GetAllSellerOrdersOfSeller();
                return Ok(ApiResponse.SuccessResponse("Orders fetched successfully.", sellerOrders));
            }

            return Forbid("Route Forbidden.");
        }

        [Authorize(Roles = AppRoles.Customer)]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto order)
        {
            var result = await orderService.CreateOrderAsync(order);
            if (result is null) return BadRequest(ApiResponse.ErrorResponse("Order creation failed.", null));
            return Ok(ApiResponse.SuccessResponse("Order created.", result));
        }

        [Authorize(Roles = AppRoles.Customer)]
        [HttpPost("{id}/Feedback")]
        public async Task<IActionResult> SubmitFeedback(int id, FeedbackDto feedbackDto)
        {
            // SellerOrder must exist
            if (!(await orderService.SellerOrderExistAsync(id)))
                return NotFound(ApiResponse.ErrorResponse("Order not found.", null));

            // SellerOrder must belong to the customer
            if (!(await orderService.SellerOrderBelongsToCustomer(id)))
                return Unauthorized(ApiResponse.ErrorResponse("You are not authorized to access this resource.", null));

            // SellerOrder status must be Delivered
            if (!(await orderService.IsSellerOrderStatusDeliveredAsync(id)))
                return BadRequest(ApiResponse.ErrorResponse("Feedback can only be given for orders that have been delivered.", null));

            var result = await orderService.SubmitFeedback(id, feedbackDto);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("Feedback submission failed.", null));
            return Ok(ApiResponse.SuccessResponse("Feedback submitted.", null));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSellerOrderStatus(int id, UpdateSellerOrderStatusDto updateSellerOrderStatusDto)
        {
            // SellerOrder must exist
            if (!(await orderService.SellerOrderExistAsync(id)))
                return NotFound(ApiResponse.ErrorResponse("Seller order not found.", null));

            if (user.IsInRole(AppRoles.Seller))
            {
                // SellerOrder must belong to the seller
                if (!(await orderService.SellerOrderBelongsToSeller(id)))
                    return Unauthorized(ApiResponse.ErrorResponse("You are not authorized to access this resource.", null));

                // Seller can only update status to Pending if it's currently Processing and vice versa.
                var result = await orderService.UpdateSellerOrderStatusForSeller(id, updateSellerOrderStatusDto);
                if (!result) return BadRequest(ApiResponse.ErrorResponse("Order status update failed.", null));
                return Ok(ApiResponse.SuccessResponse("Order status updated.", null));
            }
            
            if (user.IsInRole(AppRoles.Customer))
            {
                // SellerOrder must belong to the customer
                if (!(await orderService.SellerOrderBelongsToCustomer(id)))
                    return Unauthorized(ApiResponse.ErrorResponse("You are not authorized to update this order status.", null));

                // Customer can only cancel the order
                var result = await orderService.UpdateSellerOrderStatusForCustomer(id, updateSellerOrderStatusDto);
                if (!result) return BadRequest(ApiResponse.ErrorResponse("Order status update failed.", null));
                return Ok(ApiResponse.SuccessResponse("Order status updated.", null));
            }

            if (user.IsInRole(AppRoles.Admin))
            {
                // Admin can update any order to any status
                var result = await orderService.UpdateSellerOrderStatusForAdmin(id, updateSellerOrderStatusDto);
                if (!result) return BadRequest(ApiResponse.ErrorResponse("Order status update failed.", null));
                return Ok(ApiResponse.SuccessResponse("Order status updated.", null));
            }

            return Forbid("Route Forbidden.");
        }
    }
}
