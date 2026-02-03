using EcommerceApp.Application.Dtos;
using FluentValidation;

namespace EcommerceApp.Application.Validators
{
    public class UpdateSellerOrderStatusDtoValidator : AbstractValidator<UpdateSellerOrderStatusDto>
    {
        public UpdateSellerOrderStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid order status.");
            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
        }
    }
    public class OrderDtoValidator : AbstractValidator<OrderDto>
    {
        public OrderDtoValidator()
        {
            RuleForEach(x => x.SellerOrders).ChildRules(order =>
            {
                order.RuleFor(o => o.ProductId)
                    .NotEmpty().WithMessage("Product Id is required.")
                    .GreaterThan(0).WithMessage("Product Id must be greater than 0.");
                order.RuleFor(o => o.Quantity)
                    .NotEmpty().WithMessage("Quantity is required.")
                    .GreaterThan(0).WithMessage(o => $"Quantity of Product {o.ProductId} must be greater than 0.");
            });
        }
    }
}
