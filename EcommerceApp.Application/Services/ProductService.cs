using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork uow;
        private readonly IUserService user;
        private readonly IMapper mapper;
        public ProductService(IUnitOfWork uow, IMapper mapper, IUserService user)
        {
            this.user = user;
            this.uow = uow;
            this.mapper = mapper;
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
    }
}
