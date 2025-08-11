using DotNetSkills.Application.UserManagement.Features.CheckUserExists;

namespace DotNetSkills.Application.UserManagement.Features.UpdateUserRole;

/// <summary>
/// FluentValidation validator for UpdateUserRoleCommand.
/// Provides validation for role changes including business rule checks and authorization.
/// </summary>
public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{
    private readonly IMediator _mediator;

    public UpdateUserRoleCommandValidator(IMediator mediator)
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

        // Role validation - enum is already strongly typed, but validate it's a valid value
        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Role must be a valid UserRole (Admin, ProjectManager, Developer, or Viewer)");

        // ChangedById validation - required for authorization
        RuleFor(x => x.ChangedById.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("Changed by user ID cannot be empty");
            
        RuleFor(x => x.ChangedById)
            .MustAsync(UserExistsAsync)
            .WithMessage("User performing the change does not exist");

        // Business rule validation - users cannot change their own role
        RuleFor(x => x)
            .Must(NotChangingSelfRole)
            .WithMessage(ValidationMessages.User.CannotChangeSelfRole);

        // Additional business rule validation can be added here
        // Note: Admin-only role change validation is handled in the domain layer
        // through User.ChangeRole() method and BusinessRules.Authorization checks
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
            var exists = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
            return exists;
        }
        catch (Exception)
        {
            // If validation fails due to system error, allow it through
            // The handler will catch any issues during actual processing
            return true;
        }
    }

    /// <summary>
    /// Business rule validation to prevent users from changing their own role.
    /// This is a critical security validation that prevents privilege escalation.
    /// </summary>
    private static bool NotChangingSelfRole(UpdateUserRoleCommand command)
    {
        return command.UserId != command.ChangedById;
    }
}