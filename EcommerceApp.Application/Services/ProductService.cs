using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Exceptions;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Products;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        public ProductService(IUnitOfWork uow, IMapper  mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }
        //public async Task<GetProductDto?> AddProduct(ProductDto product)
        //{
        //    string productSlug = product.ProductSlug;
        //    bool doesProductExist = await uow.Products.ProductExistsAsync(productSlug);
        //    if (doesProductExist) return null;

        //    Product NewProduct = mapper.Map<Product>(product);
        //    NewProduct.CreatedAt = DateTime.UtcNow;
        //    NewProduct.UpdatedAt= DateTime.UtcNow;
        //    Product AddedProduct = await uow.Products.AddProduct(NewProduct);
        //    await uow.SaveChangesAsync();

        //    return mapper.Map<GetProductDto>(AddedProduct);
        //}

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

        public async Task<GetProductDto?> UpdateProduct(int id, ProductDto product)
        {
            Product? existingProduct = await uow.Products.GetProductById(id);
            if (existingProduct is null) return null;

            mapper.Map(product, existingProduct);
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await uow.SaveChangesAsync();
            return mapper.Map<GetProductDto>(existingProduct);
        }
    }
}
