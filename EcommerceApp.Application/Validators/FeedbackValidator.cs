using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;
using FluentValidation;

namespace EcommerceApp.Application.Validators
{
    public class FeedbackValidator : AbstractValidator<FeedbackDto>
    {
        public readonly IUnitOfWork uow;
        public readonly ICurrentUser user;
        public FeedbackValidator(IUnitOfWork uow, ICurrentUser user)
        {
            this.uow = uow;
            this.user = user;

            RuleFor(f => f.SellerOrderId)
                .GreaterThan(0).WithMessage("SellerOrderId must be greater than 0.");
            RuleFor(f => f.Comment)
                .NotEmpty().WithMessage("Comment is required.")
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
            RuleFor(f => f.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
            
            // SellerOrder must be in delivered status
            RuleFor(f => f.SellerOrderId)
                .MustAsync(async (id, cancellation) =>
                {
                    var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(id);
                    return sellerOrder?.Status == OrderStatus.Delivered;
                })
                .WithMessage("Feedback can only be given for orders that have been delivered.");
            
            // SellerOrder must belong to the user
            RuleFor(f => f.SellerOrderId)
                .MustAsync(async (id, cancellation) =>
                {
                    var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(id);
                    if (sellerOrder is null) return false;
                    var parentOrder = await uow.Orders.GetByIdAsync(sellerOrder.OrderId);

                    return parentOrder?.UserId == user.GetUserIdInt();

                    // This is wrong as EF does not directly include navigation properties we have to explicitly add them while fetching from db.
                    //return sellerOrder?.CorresponingOrder.UserId == user.GetUserIdInt();
                })
                .WithMessage("Seller Order does not belong to you.");
        }
    }
}
