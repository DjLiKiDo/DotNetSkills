using DotNetSkills.Application.UserManagement.Features.ValidateUserEmail;

namespace DotNetSkills.Application.UserManagement.Features.CreateUser;

/// <summary>
/// FluentValidation validator for CreateUserCommand.
/// Provides comprehensive validation including email format, uniqueness checks, and business rules.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IMediator _mediator;

    public CreateUserCommandValidator(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        // Name validation
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationMessages.User.NameRequired)
            .Length(2, 100)
            .WithMessage(string.Format(ValidationMessages.Common.MustBeInRange, "User name", 2, 100))
            .Matches(@"^[a-zA-Z\s\-.']+$")
            .WithMessage("User name can only contain letters, spaces, hyphens, periods, and apostrophes");

        // Email validation with format and async uniqueness check
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationMessages.User.EmailRequired)
            .EmailAddress()
            .WithMessage(ValidationMessages.User.InvalidEmailFormat)
            .Length(1, 254)
            .WithMessage(string.Format(ValidationMessages.Common.ExceedsMaxLength, "Email address", 254))
            .MustAsync(BeUniqueEmailAsync)
            .WithMessage("Email address is already taken");

        // Role validation - enum ensures type safety; optional explicit check for defined values
        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid user role value");

        // Optional CreatedById validation
        When(x => x.CreatedById != null, () =>
        {
            RuleFor(x => x.CreatedById!.Value)
                .NotEqual(Guid.Empty)
                .WithMessage("Created by user ID cannot be empty");
        });
    }

    // String-based role validation removed; using strongly-typed enum

    /// <summary>
    /// Async validation to check if email address is unique in the system.
    /// Uses the existing ValidateUserEmailQuery for database checks.
    /// Performance optimized to avoid unnecessary database calls when email is invalid.
    /// </summary>
    private async Task<bool> BeUniqueEmailAsync(string email, CancellationToken cancellationToken)
    {
        // Skip database check if email is null, empty, or invalid format
        if (string.IsNullOrWhiteSpace(email))
            return true; // Let other validators handle empty email

        // Basic email format check before database call for performance
        if (!email.Contains('@') || email.Length > 254)
            return true; // Let other validators handle format validation

        try
        {
            // Use existing query to check email uniqueness
            var query = new ValidateUserEmailQuery(email);
            var isAvailable = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
            return isAvailable;
        }
        catch (Exception)
        {
            // If validation fails due to system error, allow it through
            // The handler will catch any issues during actual processing
            return true;
        }
    }
}