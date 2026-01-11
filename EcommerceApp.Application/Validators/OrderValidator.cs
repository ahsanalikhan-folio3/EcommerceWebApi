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
        }
    }
}
