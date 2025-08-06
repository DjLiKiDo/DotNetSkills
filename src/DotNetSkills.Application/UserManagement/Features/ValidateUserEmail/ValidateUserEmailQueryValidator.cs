namespace DotNetSkills.Application.UserManagement.Features.ValidateUserEmail;

/// <summary>
/// Validator for ValidateUserEmailQuery.
/// </summary>
public class ValidateUserEmailQueryValidator : AbstractValidator<ValidateUserEmailQuery>
{
    public ValidateUserEmailQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(256)
            .WithMessage("Email cannot exceed 256 characters.");
    }
}
