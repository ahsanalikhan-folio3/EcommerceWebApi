using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Auth;
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
            var result = await authService.LoginUser(user);
            return Ok(result);
        }
    }
}
