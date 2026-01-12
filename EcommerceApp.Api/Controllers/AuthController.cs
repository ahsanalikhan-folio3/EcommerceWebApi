using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Auth;
using EcommerceApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        //[HttpPost("Register/Admin")]
        //public async Task<IActionResult> RegisterAdmin (AdminProfileDto user)
        //{
        //    var result = await authService.RegisterAdmin(user);
        //    if (result is null) return BadRequest(ApiResponse.ErrorResponse("User already exists.", null));
        //    return Ok(ApiResponse.SuccessResponse("User successfully added.", result));
        //}
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPatch("Users/{id}")]
        public async Task<IActionResult> ChangeUserActivationStatus(int id, [FromBody] UserActivationDto userActivationDto)
        {
            // User must exist
            var userExists = await authService.UserExistAsyncById(id);
            if (!userExists) return NotFound(ApiResponse.ErrorResponse("User does not exist.", null));

            await authService.ChangeUserActivationStatus(id, userActivationDto);
            return Ok(ApiResponse.SuccessResponse("User Activation status updated successfully.", null));
        }
        [HttpPost("Register/Customer")]
        public async Task<IActionResult> RegisterCustomer (CustomerProfileDto user)
        {
            var result = await authService.RegisterCustomer(user);
            if (result is null) return BadRequest(ApiResponse.ErrorResponse("User already exists.", null));
            return Ok(ApiResponse.SuccessResponse("Customer successfully added.", result));
        }
        [HttpPost("Register/Seller")]
        public async Task<IActionResult> RegisterSeller (SellerProfileDto user)
        {
            var result = await authService.RegisterSeller(user);
            if (result is null) return BadRequest(ApiResponse.ErrorResponse("User already exists.", null));
            return Ok(ApiResponse.SuccessResponse("Seller successfully added.", result));
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser (LoginDto user)
        {
            // User must exist
            var userExists = await authService.UserExistAsyncByEmail(user.Email);
            if (!userExists) return NotFound(ApiResponse.ErrorResponse("User does not exist.", null));

            // User must be active
            var isActive = await authService.UserIsActiveAsync(user.Email);
            if (!isActive) return BadRequest(ApiResponse.ErrorResponse("User is deactivated.", null));

            // Password must be correct
            var isPasswordCorrect = await authService.ValidatePassword(user);
            if (!isPasswordCorrect) return BadRequest(ApiResponse.ErrorResponse("Invalid password.", null));

            var result = await authService.LoginUser(user);
            return Ok(ApiResponse.SuccessResponse("User successfully logged in.", result));
        }
        [Authorize]
        [HttpGet("Users/Profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var result = await authService.GetUserProfileAsync();
            return Ok(ApiResponse.SuccessResponse("User profile retrieved successfully.", result));
        }
    }
}
