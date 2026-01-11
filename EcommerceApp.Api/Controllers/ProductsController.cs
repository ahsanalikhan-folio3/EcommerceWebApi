using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IUserService user;
        public ProductsController(IProductService productService, IUserService user)
        {
            this.user = user;
            this.productService = productService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] bool mine) 
        {
            // If mine is true and user is Seller send his products only.
            if (mine && user.IsInRole(AppRoles.Seller))
            {
                var sellerProducts = await productService.GetAllSellerProducts();
                return Ok(ApiResponse.SuccessResponse("Fetched all products.", sellerProducts));
            }

            // For other roles send all products.
            List<GetProductDto> productsData = await productService.GetAllProducts();
            return Ok(ApiResponse.SuccessResponse("Fetched all products.", productsData));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByid (int id) 
        {
            // Fetch product by id
            GetProductDto? product = await productService.GetProductById(id);
            if (product is null) return NotFound(ApiResponse.ErrorResponse("Product not found.", null));
            return Ok(ApiResponse.SuccessResponse("Product found.", product));
        }

        [Authorize(Roles = AppRoles.Seller)]
        [HttpPost]
        public async Task<IActionResult> AddProduct (ProductDto product) 
        {
            // Only sellers can add products
            GetProductDto? resultant = await productService.AddProduct(product);
            if (resultant is null) return BadRequest(ApiResponse.ErrorResponse("Product already exist.", null));
            return Ok(ApiResponse.SuccessResponse("Product added successfully.", resultant));
        }

        [Authorize(Roles = AppRoles.Seller)]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct (int id, ProductDto product) 
        {
            // Check if product belongs to the seller
            if (!(await productService.ProductBelongsToSellerAsync(id)))
                return Unauthorized(ApiResponse.ErrorResponse("You are not authorized to access this resource.", null));

            // Only sellers can update products
            GetProductDto? resultant = await productService.UpdateProduct(id, product);
            if (resultant is null) return NotFound(ApiResponse.ErrorResponse("Product not found.", null));
            return Ok(ApiResponse.SuccessResponse("Product updated successfully.", resultant));
        }

        [Authorize(Roles = AppRoles.Seller)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct (int id) 
        {
            // Check if product belongs to the seller
            if (!(await productService.ProductBelongsToSellerAsync(id)))
                return Unauthorized(ApiResponse.ErrorResponse("You are not authorized to access this resource.", null));

            // Only sellers can delete products
            bool result = await productService.DeleteProduct(id);
            if (!result) return NotFound(ApiResponse.ErrorResponse("Product not found.", null));
            return Ok(ApiResponse.SuccessResponse("Product deleted successfully.", null));
        }

        [Authorize(Roles = AppRoles.Seller)]
        [HttpGet("{id}/Feedbacks")]
        public async Task<IActionResult> GetProductFeedbacks (int id) 
        {
            // Check if product belongs to the seller
            if (!(await productService.ProductBelongsToSellerAsync(id)))
                return Unauthorized(ApiResponse.ErrorResponse("You are not authorized to access this resource.", null));

            // Only sellers can view feedbacks
            var feedbacks = await productService.GetProductFeedbacks(id);
            if (feedbacks is null) return NotFound(ApiResponse.ErrorResponse("Invalid id or Product not found.", null));
            return Ok(ApiResponse.SuccessResponse("Product feedbacks fetched successfully.", feedbacks));
        }

        [Authorize(Roles = AppRoles.Seller)]
        [HttpGet("{id}/Orders")]
        public async Task<IActionResult> GetProductOrders (int id) 
        {
            // Check if product belongs to the seller
            if (!(await productService.ProductBelongsToSellerAsync(id)))
                return Unauthorized(ApiResponse.ErrorResponse("You are not authorized to access this resource.", null));

            // Only sellers can view orders
            var orders = await productService.GetProductOrders(id);
            if (orders is null) return NotFound(ApiResponse.ErrorResponse("Invalid id or Product not found.", null));
            return Ok(ApiResponse.SuccessResponse("Product orders fetched successfully.", orders));
        }
    }
}
