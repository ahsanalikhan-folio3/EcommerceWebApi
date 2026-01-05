using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;
using FluentValidation;

namespace EcommerceApp.Application.Validators
{
    public class CancelOrderDtoValidator : AbstractValidator<CancelOrderDto>
    {
        private readonly IUnitOfWork uow;
        private readonly ICurrentUser currentUser;
        public CancelOrderDtoValidator(IUnitOfWork uow, ICurrentUser currentUser)
        {
            this.uow = uow;
            this.currentUser = currentUser;

            RuleFor(x => x.SellerOrderId)
                .GreaterThan(0).WithMessage("OrderId must be greater than zero.");
            
            // Order must belong to the user
            RuleFor(x => x.SellerOrderId)
                .MustAsync(async (id, cancellation) =>
                {
                    var sellerOrder = await uow.SellerOrders.GetSellerOrdersById(id);
                    if (sellerOrder == null)
                        return false;

                    var parentOrder = await uow.Orders.GetByIdAsync(sellerOrder.OrderId);
                    if (parentOrder == null)
                        return false;

                    int currentUserId = int.Parse(currentUser.UserId!);

                    return parentOrder.UserId == currentUserId;
                })
                .WithMessage("Order does not belong to the current user.");

            // Order must be in pending status
            RuleFor(x => x.SellerOrderId)
                .MustAsync(async (id, cancellation) =>
                {
                    var order = await uow.SellerOrders.GetSellerOrdersById(id);
                    return order != null && order.Status == OrderStatus.Pending;
                })
                .WithMessage("Only orders with pending status can be cancelled.");
        }
    }
}
