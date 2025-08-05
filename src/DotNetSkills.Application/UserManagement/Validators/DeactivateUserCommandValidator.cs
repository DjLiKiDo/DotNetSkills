namespace DotNetSkills.Application.UserManagement.Validators;

/// <summary>
/// FluentValidation validator for DeactivateUserCommand.
/// Provides validation for user deactivation including business rule checks and authorization.
/// </summary>
public class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    private readonly IMediator _mediator;

    public DeactivateUserCommandValidator(IMediator mediator)
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

        // DeactivatedById validation - required for authorization
        RuleFor(x => x.DeactivatedById.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("Deactivated by user ID cannot be empty");
            
        RuleFor(x => x.DeactivatedById)
            .MustAsync(UserExistsAsync)
            .WithMessage("User performing the deactivation does not exist");

        // Business rule validation - users cannot deactivate themselves
        // This prevents users from locking themselves out of the system
        RuleFor(x => x)
            .Must(NotDeactivatingSelf)
            .WithMessage("Users cannot deactivate themselves");

        // Note: Admin-only deactivation validation is handled in the domain layer
        // through User.Deactivate() method and BusinessRules.Authorization checks
        // This follows DDD principles where complex business rules are in the domain
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
    /// Business rule validation to prevent users from deactivating themselves.
    /// This is a critical security validation that prevents users from locking themselves out.
    /// </summary>
    private static bool NotDeactivatingSelf(DeactivateUserCommand command)
    {
        return command.UserId != command.DeactivatedById;
    }
}