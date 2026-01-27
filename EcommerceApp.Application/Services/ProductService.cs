using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace EcommerceApp.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork uow;
        private readonly IUserService user;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;
        public ProductService(IUnitOfWork uow, IMapper mapper, IUserService user, IWebHostEnvironment env, IConfiguration configuration)
        {
            this.user = user;
            this.uow = uow;
            this.mapper = mapper;
            this.env = env;
            this.configuration = configuration;
        }
        public async Task<bool> ProductBelongsToSellerAsync(int productId)
        {
            var product = await uow.Products.GetProductById(productId);
            return (product?.SellerId == user.GetUserIdInt()) ? true : false;
        }
        public async Task<GetProductDto?> AddProduct(ProductDto product)
        {
            string productSlug = product.ProductSlug;
            bool doesProductExist = await uow.Products.ProductExistsAsync(productSlug);
            if (doesProductExist) return null;

            Product NewProduct = mapper.Map<Product>(product);
            NewProduct.SellerId = user.GetUserIdInt();
            NewProduct.CreatedAt = DateTime.UtcNow;
            NewProduct.UpdatedAt = DateTime.UtcNow;
            Product AddedProduct = await uow.Products.AddProduct(NewProduct);
            await uow.SaveChangesAsync();

            return mapper.Map<GetProductDto>(AddedProduct);
        }
        public async Task<GetProductDto?> UpdateProduct(int productId, ProductDto product)
        {
            var dbProduct = await uow.Products.GetProductById(productId);
            if (dbProduct is null) return null;

            mapper.Map(product, dbProduct);

            dbProduct.UpdatedAt = DateTime.UtcNow;
            await uow.SaveChangesAsync();

            return mapper.Map<GetProductDto>(dbProduct);
        }
        public async Task<List<GetProductDto>> GetAllSellerProducts()
        {
            int sellerId = user.GetUserIdInt();
            var products = await uow.Products.GetAllSellerProducts(sellerId);
            return mapper.Map<List<GetProductDto>>(products);
        }

        public async Task<bool> DeleteProduct(int id)
        {
            Product? product = await uow.Products.GetProductById(id);
            if (product is null) return false;

            bool result = await uow.Products.DeleteProductById(id);
            await uow.SaveChangesAsync();

            return result;
        }
        public async Task<List<GetProductDto>> GetAllProducts()
        {
            List<Product> products = await uow.Products.GetAllProducts();
            return mapper.Map<List<GetProductDto>>(products);
        }

        public async Task<GetProductDto?> GetProductById(int id)
        {
            Product? product = await uow.Products.GetProductById(id);
            return mapper.Map<GetProductDto>(product);
        }

        public async Task<List<GetFeedbackDto>?> GetProductFeedbacks(int productId)
        {
            if (productId <= 0 || !await ProductExistsAsync(productId)) return null;

            int sellerId = user.GetUserIdInt();

            var allFeedbacksOfSeller =
                await uow.Feedbacks.GetAllFeedbacksOfSeller(sellerId);

            var allOrdersOfProduct =
                await uow.SellerOrders.GetAllSellerOrdersOfProduct(productId);

            var productOrderIds = allOrdersOfProduct
                .Select(o => o.Id)
                .ToHashSet();

            var requiredFeedbacks = allFeedbacksOfSeller
                .Where(f => productOrderIds.Contains(f.SellerOrderId))
                .Select(f => mapper.Map<GetFeedbackDto>(f))
                .ToList();

            return requiredFeedbacks;
        }

        public async Task<List<GetSellerOrderDto>?> GetProductOrders(int productId)
        {
            if (productId <= 0 || !await ProductExistsAsync(productId)) return null;

            var sellerOrders = await uow.SellerOrders.GetAllSellerOrdersOfProduct(productId);
            return mapper.Map<List<GetSellerOrderDto>>(sellerOrders);
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            var product = await uow.Products.GetProductById(productId);
            return product is not null;
        }

        public async Task<bool> AddProductImages(int productId, ProductImageUploadDto productImageUploadDto)
        {
            var imageFiles = productImageUploadDto.ImageFiles;
            var product = await uow.Products.GetProductById(productId);
            ICollection<ProductImage> productImages = new List<ProductImage>();

            int initialCount = await uow.ProductImages.GetProductImagesCountByProductId(productId);
            int ProductImageslimit = Convert.ToInt32(configuration["MaxLimits:ProductImages"] ?? "2");

            // Check if adding these images would exceed the limit
            if (initialCount + imageFiles.Count > ProductImageslimit)
                return false;

            ICollection<string> allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif" };
            long maxBytes = Convert.ToInt64(
                configuration["MaxLimits:ProductImageInBytes"] ?? "1048576"
            );

            // Any invalid file found, return false
            foreach (var imageFile in imageFiles)
            {
                // Validate file extension
                string extension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    return false;
                // Validate file size
                if (imageFile.Length > maxBytes)
                    return false;
            }

            foreach (var imageFile in imageFiles)
            {
                // Logic to upload and associate image with product
                DateTime createdAt = DateTime.UtcNow;
                string extension = Path.GetExtension(imageFile.FileName).ToLower();
                string fileName = $"{Guid.NewGuid()}{extension}"; 
                string webRootPath = env.WebRootPath;
                string directoryPath = Path.Combine(webRootPath, "images", "products", productId.ToString());

                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                string filePath = Path.Combine(directoryPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await imageFile.CopyToAsync(fileStream);

                productImages.Add(new ProductImage() { ProductId = productId, CreatedAt = createdAt, ImageUrl = $"/images/products/{productId}/{fileName}" });
            }

            await uow.ProductImages.AddProductImages(productImages);
            await uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductImage(int imageId)
        {
            var productImage = await uow.ProductImages.GetProductImageById(imageId);
            if (productImage is null) return false;

            string storedPath = productImage.ImageUrl.TrimStart('/');
            string filePath = Path.Combine(env.WebRootPath, storedPath);

            if (File.Exists(filePath)) File.Delete(filePath);

            // Delete the Db record as well.
            await uow.ProductImages.DeleteProductImageById(imageId);
            await uow.SaveChangesAsync();

            return true;
        }
    }
}
