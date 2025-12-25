using EcommerceApp.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = AppRoles.Admin)]
        public IActionResult TestAdminRoute ()
        {
            return Ok("You are an Admin.");
        }
    }
}
