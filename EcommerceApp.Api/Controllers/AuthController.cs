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

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser (RegisterDto user)
        {
            var result = await authService.RegisterUser(user);
            if (result is null) return BadRequest(ApiResponse.ErrorResponse("User already exists.", null));
            return Ok(ApiResponse.SuccessResponse("User successfully added.", result));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser (LoginDto user)
        {
            var result = await authService.LoginUser(user);
            return Ok(result);
        }
    }
}
