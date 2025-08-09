namespace DotNetSkills.Application.UserManagement.Features.DeleteUser;

/// <summary>
/// Validator for DeleteUserCommand.
/// </summary>
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.UserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("UserId cannot be empty GUID.");
    }
}
