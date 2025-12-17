using EcommerceApp.Application.Interfaces.Products;

namespace EcommerceApp.Application.Interfaces
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; set; }
        public Task SaveChangesAsync();
    }
}
