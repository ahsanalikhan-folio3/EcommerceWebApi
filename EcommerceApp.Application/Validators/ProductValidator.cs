using EcommerceApp.Application.Dtos;
using FluentValidation;

namespace EcommerceApp.Application.Validators
{
    public class ProductValidator : AbstractValidator<ProductDto>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3).WithMessage("Name must have a min length of 3")
                .MaximumLength(150).WithMessage("Name too long");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(10).WithMessage("Description must have a min length of 10")
                .MaximumLength(500).WithMessage("Description too long");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("StockQuantity must be 0 or greater");

            RuleFor(x => x.ProductSlug)
                .NotEmpty().WithMessage("ProductSlug is required")
                .MaximumLength(100)
                .Matches("^[a-z0-9-]+$").WithMessage("ProductSlug must be lowercase and hyphen-separated");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required");
        }
    }
    public class UpdateProductStockDtoValidator : AbstractValidator<UpdateProductStockDto>
    {
        public UpdateProductStockDtoValidator()
        {
            RuleFor(x => x.Stock)
                .GreaterThan(-1).WithMessage("Stock must be greater than or equal to 0.");
        }
    }
}
