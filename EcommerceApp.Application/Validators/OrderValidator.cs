using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.User;
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
