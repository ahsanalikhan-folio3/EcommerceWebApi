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

            RuleFor(f => f.Comment)
                .NotEmpty().WithMessage("Comment is required.")
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
            RuleFor(f => f.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        }
    }
}
