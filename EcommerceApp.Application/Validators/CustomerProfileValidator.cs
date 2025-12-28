using EcommerceApp.Domain.Entities;
using FluentValidation;

namespace EcommerceApp.Application.Validators
{
    public class CustomerProfileValidator : AbstractValidator<CustomerProfile>
    {
        public CustomerProfileValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.HouseNumber)
                .NotEmpty().WithMessage("House number is required.")
                .MaximumLength(20);

            RuleFor(x => x.StreetNumber)
                .NotEmpty().WithMessage("Street number is required.")
                .MaximumLength(50);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100);

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100);

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Postal code is required.")
                .MaximumLength(20)
                .Matches(@"^[A-Za-z0-9\- ]+$")
                .WithMessage("Postal code format is invalid.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100);

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(BeValidGender)
                .WithMessage("Gender must be Male, Female, or Other.");
        }

        private bool BeValidGender(string gender)
        {
            return gender.Equals("Male", StringComparison.OrdinalIgnoreCase)
                || gender.Equals("Female", StringComparison.OrdinalIgnoreCase)
                || gender.Equals("Other", StringComparison.OrdinalIgnoreCase);
        }
    }
}

