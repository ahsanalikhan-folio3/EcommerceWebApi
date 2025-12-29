using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Sellers;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService SellerService;
        public SellerController(ISellerService SellerService)
        {
            this.SellerService = SellerService;
        }
        //[HttpPost("{id}")]
        //public async Task<IActionResult> CreateSellerProfile(Guid id, SellerProfileDto SellerProfileDto)
        //{
        //    var result = await SellerService.AddSellerProfile(id, SellerProfileDto);
        //    if (!result) return BadRequest(ApiResponse.ErrorResponse("Seller Profile already exists", null));
        //    return Ok(ApiResponse.SuccessResponse("Seller Profile successfully created", null));
        //}
    }
}
