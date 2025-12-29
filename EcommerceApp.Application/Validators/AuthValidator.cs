using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using FluentValidation;

namespace EcommerceApp.Application.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            // Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email address is not valid.");

            // Password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.")
                .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d+").WithMessage("Password must contain at least one number.")
                .Matches(@"[!@#$%^&*()\-+=_\[\]{}|;:'"",<.>/?]+")
                .WithMessage("Password must contain at least one special character.");

            // ConfirmPassword
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            // Phone Number
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Phone number is not valid.");

            // Role
            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(role => AppRoles.AllRoles.Contains(role))
                .WithMessage($"Role must be one of the following: {string.Join(", ", AppRoles.AllRoles)}");

            // Full Name
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MinimumLength(3).WithMessage("Full name must have a minimum length of 3.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        }
    }
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            // Email
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email Address is not valid");
        }
    }

    public class CustomerProfileDtoValidator : AbstractValidator<CustomerProfileDto>
    {
        public CustomerProfileDtoValidator() 
        {
            Include(new RegisterUserDtoValidator());

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
    public class SellerProfileDtoValidator : AbstractValidator<SellerProfileDto>
    {
        public SellerProfileDtoValidator()
        {
            Include(new RegisterUserDtoValidator());
            RuleFor(x => x.Storename)
           .NotEmpty().WithMessage("Store name is required.")
           .MinimumLength(3).WithMessage("Store name must be at least 3 characters.")
           .MaximumLength(100).WithMessage("Store name must not exceed 100 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100);

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100);

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Postal code is required.")
                .Matches(@"^[A-Za-z0-9\- ]{3,12}$")
                .WithMessage("Postal code format is invalid.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100);
        }

        private bool BeValidGender(string gender)
        {
            return gender.Equals("Male", StringComparison.OrdinalIgnoreCase)
                || gender.Equals("Female", StringComparison.OrdinalIgnoreCase)
                || gender.Equals("Other", StringComparison.OrdinalIgnoreCase);
        }
    }

}
