using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Sellers;
using EcommerceApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Seller)]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService sellerService;
        public SellerController(ISellerService sellerService)
        {
            this.sellerService = sellerService;
        }

        [HttpPut("Order/Status")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateSellerOrderStatusFromSellerSideDto UpdateSellerOrderStatusFromSellerSideDto)
        {
            var result = await sellerService.UpdateOrderStatus(UpdateSellerOrderStatusFromSellerSideDto);
            if (!result) return BadRequest(ApiResponse.ErrorResponse("Failed to update order status", null));
            return Ok(ApiResponse.SuccessResponse("Order status successfully updated", null));
        }

        [HttpGet("Products")]
        public async Task<IActionResult> GetAllSellerProduct()
        {
            var products = await sellerService.GetAllSellerProducts();
            if (products is null) return NotFound(ApiResponse.ErrorResponse("Products not found.", null));
            return Ok(ApiResponse.SuccessResponse("Product fetched successfully.", products));
        }

        [HttpPost("Products")]
        public async Task<IActionResult> AddProduct(ProductDto product)
        {
            GetProductDto? resultant = await sellerService.AddProduct(product);
            if (resultant is null) return BadRequest(ApiResponse.ErrorResponse("Product already exist.", null));
            return Ok(ApiResponse.SuccessResponse("Product added successfully.", resultant));
        }

        [HttpPut("Products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto product)
        {
            var result = await sellerService.UpdateProduct(id, product);
            if (result is null) return BadRequest(ApiResponse.ErrorResponse("Failed to update product.", null));
            return Ok(ApiResponse.SuccessResponse("Product successfully updated", result));
        }
    }
}
