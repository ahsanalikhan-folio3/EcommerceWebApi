using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.User;
using FluentValidation;

namespace EcommerceApp.Application.Validators
{
    public class FeedbackValidator : AbstractValidator<FeedbackDto>
    {
        public readonly IUnitOfWork uow;
        public readonly IUserService user;
        public FeedbackValidator(IUnitOfWork uow, IUserService user)
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
