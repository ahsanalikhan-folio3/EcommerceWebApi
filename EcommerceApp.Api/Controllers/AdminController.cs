using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Admins;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService adminProfileService;
        public AdminController(IAdminService adminProfileService)
        {
            this.adminProfileService = adminProfileService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAdminProfile(AdminProfileDto adminProfileDto)
        {
            var result = await adminProfileService.AddAdminProfile(adminProfileDto);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("Admin Profile already exists", null));
            return Ok(ApiResponse.SuccessResponse("Admin Profile successfully created", null));
        }
        [HttpPost("activate-user")]
        public async Task<IActionResult> ActivateUser(Guid UserId)
        {
            bool result = await adminProfileService.ActivateUser(UserId);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("User does not exist or is already active", null));
            return Ok(ApiResponse.SuccessResponse("User successfully activated", null));
        }
        [HttpPost("deactivate-user")]
        public async Task<IActionResult> DeActivateUser(Guid UserId)
        {
            bool  result = await adminProfileService.DeActivateUser(UserId);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("User does not exist or is already inactive", null));
            return Ok(ApiResponse.SuccessResponse("User successfully deactivated", null));
        }
    }
}
