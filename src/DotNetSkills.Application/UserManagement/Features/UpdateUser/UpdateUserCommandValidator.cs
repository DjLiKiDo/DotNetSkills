using DotNetSkills.Application.UserManagement.Features.CheckUserExists;
using DotNetSkills.Application.UserManagement.Features.ValidateUserEmail;

namespace DotNetSkills.Application.UserManagement.Features.UpdateUser;

/// <summary>
/// FluentValidation validator for UpdateUserCommand.
/// Provides validation for user profile updates including name and email with uniqueness checks.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IMediator _mediator;

    public UpdateUserCommandValidator(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        // UserId validation
        RuleFor(x => x.UserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID cannot be empty");
            
        RuleFor(x => x.UserId)
            .MustAsync(UserExistsAsync)
            .WithMessage("User does not exist");

        // Name validation
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationMessages.User.NameRequired)
            .Length(2, 100)
            .WithMessage(string.Format(ValidationMessages.Common.MustBeInRange, "User name", 2, 100))
            .Matches(@"^[a-zA-Z\s\-.']+$")
            .WithMessage("User name can only contain letters, spaces, hyphens, periods, and apostrophes");

        // Email validation with format and async uniqueness check (excluding current user)
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationMessages.User.EmailRequired)
            .EmailAddress()
            .WithMessage(ValidationMessages.User.InvalidEmailFormat)
            .Length(1, 254)
            .WithMessage(string.Format(ValidationMessages.Common.ExceedsMaxLength, "Email address", 254))
            .MustAsync(BeUniqueEmailAsync)
            .WithMessage("Email address is already taken by another user");

        // Optional UpdatedById validation
        When(x => x.UpdatedById != null, () =>
        {
            RuleFor(x => x.UpdatedById!.Value)
                .NotEqual(Guid.Empty)
                .WithMessage("Updated by user ID cannot be empty");
        });
    }

    /// <summary>
    /// Validates that the user exists in the system.
    /// Uses the existing CheckUserExistsQuery for database checks.
    /// </summary>
    private async Task<bool> UserExistsAsync(UserId userId, CancellationToken cancellationToken)
    {
        try
        {
            var query = new CheckUserExistsQuery(userId);
            var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

            return result.IsSuccess && result.Value;
        }
        catch (Exception)
        {
            // If validation fails due to system error, allow it through
            // The handler will catch any issues during actual processing
            return true;
        }
    }

    /// <summary>
    /// Async validation to check if email address is unique in the system, excluding the current user.
    /// Uses the existing ValidateUserEmailQuery with ExcludeUserId parameter.
    /// Performance optimized to avoid unnecessary database calls when email is invalid.
    /// </summary>
    private async Task<bool> BeUniqueEmailAsync(UpdateUserCommand command, string email, CancellationToken cancellationToken)
    {
        // Skip database check if email is null, empty, or invalid format
        if (string.IsNullOrWhiteSpace(email))
            return true; // Let other validators handle empty email

        // Basic email format check before database call for performance
        if (!email.Contains('@') || email.Length > 254)
            return true; // Let other validators handle format validation

        try
        {
            // Use existing query to check email uniqueness, excluding the current user
            var query = new ValidateUserEmailQuery(email, command.UserId);
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