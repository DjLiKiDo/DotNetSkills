namespace DotNetSkills.Application.UserManagement.Features.ActivateUser;

/// <summary>
/// Validator for ActivateUserCommand.
/// </summary>
public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.UserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("UserId cannot be empty GUID.");
    }
}
