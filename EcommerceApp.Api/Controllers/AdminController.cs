using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Admins;
using EcommerceApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService adminService;
        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateAdminProfile(AdminProfileDto adminProfileDto)
        //{
        //    var result = await adminProfileService.AddAdminProfile(adminProfileDto);
        //    if (!result) return BadRequest(ApiResponse.ErrorResponse("Admin Profile already exists", null));
        //    return Ok(ApiResponse.SuccessResponse("Admin Profile successfully created", null));
        //}
        [HttpPost("Activate-User")]
        public async Task<IActionResult> ActivateUser(int UserId)
        {
            bool result = await adminService.ActivateUser(UserId);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("User does not exist or is already active", null));
            return Ok(ApiResponse.SuccessResponse("User successfully activated", null));
        }
        [HttpPost("Deactivate-User")]
        public async Task<IActionResult> DeActivateUser(int UserId)
        {
            bool  result = await adminService.DeActivateUser(UserId);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("User does not exist or is already inactive", null));
            return Ok(ApiResponse.SuccessResponse("User successfully deactivated", null));
        }
        [HttpPut("Order/Status")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateSellerOrderStatusFromAdminSideDto updateSellerOrderStatusFromAdminSide)
        {
            var result = await adminService.UpdateSellerOrderStatus(updateSellerOrderStatusFromAdminSide);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("Failed to update order status", null));
            return Ok(ApiResponse.SuccessResponse("Order status successfully updated", null));
        }
    }
}
