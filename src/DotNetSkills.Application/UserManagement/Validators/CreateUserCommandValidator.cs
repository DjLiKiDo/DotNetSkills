namespace DotNetSkills.Application.UserManagement.Validators;

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

        // Role validation - using EmailAddress value object validation approach
        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("User role is required")
            .Must(BeValidUserRole)
            .WithMessage("Role must be Admin, ProjectManager, Developer, or Viewer");

        // Optional CreatedById validation
        When(x => x.CreatedById != null, () =>
        {
            RuleFor(x => x.CreatedById!.Value)
                .NotEqual(Guid.Empty)
                .WithMessage("Created by user ID cannot be empty");
        });
    }

    /// <summary>
    /// Validates that the role string can be parsed to a valid UserRole enum value.
    /// Uses the same validation approach as the EmailAddress value object.
    /// </summary>
    private static bool BeValidUserRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        try
        {
            // Attempt to parse the role to UserRole enum
            return Enum.TryParse<UserRole>(role, ignoreCase: true, out _);
        }
        catch
        {
            return false;
        }
    }

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
            var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

            // Return true if email is available (query returns true for available emails)
            return result.IsSuccess && result.Value;
        }
        catch (Exception)
        {
            // If validation fails due to system error, allow it through
            // The handler will catch any issues during actual processing
            return true;
        }
    }
}