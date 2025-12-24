using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using FluentValidation;

namespace EcommerceApp.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            // Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email address is not valid.");

            // Password
            RuleFor(x => x.HashedPassword)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.")
                .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d+").WithMessage("Password must contain at least one number.")
                .Matches(@"[!@#$%^&*()\-+=_\[\]{}|;:'"",<.>/?]+")
                .WithMessage("Password must contain at least one special character.");

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
}
