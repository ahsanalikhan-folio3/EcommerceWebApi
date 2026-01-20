using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using FluentValidation;

namespace EcommerceApp.Application.Validators
{
    public class CreateChatDtoValidator : AbstractValidator<CreateChatDto>
    {
        private readonly IUnitOfWork uow;
        public CreateChatDtoValidator(IUnitOfWork uow)
        {
            this.uow = uow;
            RuleFor(x => x.SellerId)
                .GreaterThan(0).WithMessage("SellerId must be a positive integer.");
            RuleFor(x => x.SellerId)
                .NotEmpty().WithMessage("SellerId is required.");
            RuleFor(x => x.SellerId)
                .MustAsync(async (sellerId, CancellationToken) => await uow.Sellers.SellerExistAsync(sellerId)).WithMessage("Seller does not exist.");
        }
    }
}
