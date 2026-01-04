using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;
using FluentValidation;

namespace EcommerceApp.Application.Validators
{
    public class UpdateSellerOrderStatusFromSellerSideValidator : AbstractValidator<UpdateSellerOrderStatusFromSellerSideDto>
    {
        private readonly IUnitOfWork uow;
        private readonly ICurrentUser user;
        public UpdateSellerOrderStatusFromSellerSideValidator(IUnitOfWork uow, ICurrentUser user)
        {
            this.uow = uow;
            this.user = user;

            // Validate the SellerOrderId.
            RuleFor(x => x.SellerOrderId)
                .GreaterThan(0)
                .WithMessage("SellerOrderId must be greater than 0.");

            // Status must be either Pending or Processing.
            RuleFor(x => x.Status)
                .IsInEnum()
                .Must(status =>
                    status == OrderStatus.Pending ||
                    status == OrderStatus.Processing)
                .WithMessage("Status must be either Pending or Processing.");

            // Order must exist.
            RuleFor(x => x.SellerOrderId)
                .MustAsync(async (id, ct) =>
                    await uow.SellerOrders.SellerOrderExistAsync(id))
                .WithMessage("Order does not exist");

            // Order must belong to seller.
            RuleFor(x => x.SellerOrderId)
                .MustAsync(async (id, ct) =>
                    {
                        int? productId = await uow.SellerOrders.GetProductId(id);
                        if (productId is null || user.UserId is null) return false;
                        return await uow.Products.ProductBelongsToSellerAsync((int) productId, int.Parse(user.UserId));
                    })
                .WithMessage("Order does not belong to you.");
        }
    }

    public class UpdateSellerOrderStatusFromAdminSideValidator : AbstractValidator<UpdateSellerOrderStatusFromAdminSideDto>
    {
        private readonly IUnitOfWork uow;
        private readonly ICurrentUser user;
        public UpdateSellerOrderStatusFromAdminSideValidator(IUnitOfWork uow, ICurrentUser user)
        {
            this.uow = uow;
            this.user = user;

            // Validate the SellerOrderId.
            RuleFor(x => x.SellerOrderId)
                .GreaterThan(0)
                .WithMessage("SellerOrderId must be greater than 0.");

            // Status must be either InWarehouse, Shipped or Delivered.
            RuleFor(x => x.Status)
                .IsInEnum()
                .Must(status =>
                    status == OrderStatus.InWarehouse ||
                    status == OrderStatus.Shipped ||
                    status == OrderStatus.Delivered)
                .WithMessage("Status must be either InWarehouse, Shipped or Delivered.");

            // Order must exist.
            RuleFor(x => x.SellerOrderId)
                .MustAsync(async (id, ct) =>
                    await uow.SellerOrders.SellerOrderExistAsync(id))
                .WithMessage("Order does not exist");
        }
    }

}
