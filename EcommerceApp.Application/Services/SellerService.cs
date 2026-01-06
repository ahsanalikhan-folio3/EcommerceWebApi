using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Sellers;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class SellerService : ISellerService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly ICurrentUser user;
        public SellerService(IUnitOfWork uow, IMapper mapper, ICurrentUser user)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.user = user;
        }
        public async Task<bool> ProductBelongsToUserAsync (int productId)
        {
            var product = await uow.Products.GetProductById(productId);
            return (product?.SellerId == user.GetUserIdInt()) ?  true : false;
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
        public async Task<bool> UpdateOrderStatus(UpdateSellerOrderStatusFromSellerSideDto updateSellerOrderStatusFromSellerSide)
        {
            var result = await uow.SellerOrders.UpdateSellerOrderStatus(updateSellerOrderStatusFromSellerSide.SellerOrderId, updateSellerOrderStatusFromSellerSide.Status);
            await uow.SaveChangesAsync();
            return result;
        }
        public async Task<List<GetProductDto>> GetAllSellerProducts ()
        {
            int sellerId = user.GetUserIdInt();
            var products = await uow.Products.GetAllSellerProducts(sellerId);
            return mapper.Map<List<GetProductDto>>(products);
        }

        public async Task<List<GetFeedbackDto>> GetProductFeedbacks(int productId)
        {
            if (productId <= 0) return new List<GetFeedbackDto>();

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
        public async Task<List<GetSellerOrderDto>> GetProductOrders(int productId)
        {
            if (productId <= 0) return new List<GetSellerOrderDto>();

            var allSellerOrdersOfProduct = await uow.SellerOrders.GetAllSellerOrdersOfProductAlongWithProduct(productId);
            return mapper.Map<List<GetSellerOrderDto>>(allSellerOrdersOfProduct);
        }
        public async Task<List<GetSellerOrderDto>> GetProductOrdersByStatus(int productId, OrderStatus status)
        {
            if (productId <= 0) return new List<GetSellerOrderDto>();

            var allSellerOrdersOfProduct = await uow.SellerOrders.GetAllSellerOrdersOfProductAlongWithProduct(productId);
            var filteredOrders = allSellerOrdersOfProduct.Where(x => x.Status == status).ToList();

            return mapper.Map<List<GetSellerOrderDto>>(filteredOrders);
        }
    }
}
