using FluentValidation;

namespace Bookify.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(200).WithMessage("First name must be at most 200 characters.")
            .WithName("First name");

        RuleFor(c => c.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(200).WithMessage("Last name must be at most 200 characters.")
            .WithName("Last name");

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(400).WithMessage("Email must be at most 400 characters.")
            .WithName("Email");

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Password is required.")
            //.MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            //.MaximumLength(100).WithMessage("Password must be at most 100 characters.")
            //.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            //.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            //.Matches(@"\d").WithMessage("Password must contain at least one digit.")
            //.Matches(@"[\W_]").WithMessage("Password must contain at least one special character.")
            .WithName("Password");
    }
}