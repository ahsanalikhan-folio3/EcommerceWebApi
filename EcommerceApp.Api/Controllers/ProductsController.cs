using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts () 
        {
            List<GetProductDto> productsData = await productService.GetAllProducts();
            return Ok(ApiResponse.SuccessResponse("Fetched all products.", productsData));
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByid (int id) 
        {
            GetProductDto? product = await productService.GetProductById(id);
            if (product is null) return NotFound(ApiResponse.ErrorResponse("Product not found.", null));
            return Ok(ApiResponse.SuccessResponse("Product found.", product));
        }

        //[Authorize(Roles = AppRoles.Seller)]
        //[HttpPost]
        //public async Task<IActionResult> AddProduct (ProductDto product) 
        //{
        //    GetProductDto? resultant = await productService.AddProduct(product);
        //    if (resultant is null) return BadRequest(ApiResponse.ErrorResponse("Product already exist.", null));
        //    return Ok(ApiResponse.SuccessResponse("Product added successfully.", resultant));
        //}

        //    [HttpPut("{id}")]
        //    public async Task<IActionResult> UpdateProduct (int id, ProductDto product) 
        //    {
        //        GetProductDto? resultant = await productService.UpdateProduct(id, product);
        //        if (resultant is null) return NotFound(ApiResponse.ErrorResponse("Product not found.", null));
        //        return Ok(ApiResponse.SuccessResponse("Product updated successfully.", resultant));
        //    }

        //    [HttpDelete("{id}")]
        //    public async Task<IActionResult> DeleteProduct (int id) 
        //    {
        //        bool result = await productService.DeleteProduct(id);
        //        if (!result) return NotFound(ApiResponse.ErrorResponse("Product not found.", null));
        //        return Ok(ApiResponse.SuccessResponse("Product deleted successfully.", null));
        //    }
    }
}
