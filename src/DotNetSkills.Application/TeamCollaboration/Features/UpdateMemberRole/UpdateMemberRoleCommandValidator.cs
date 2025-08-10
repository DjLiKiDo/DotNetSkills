namespace DotNetSkills.Application.TeamCollaboration.Features.UpdateMemberRole;

/// <summary>
/// Validator for UpdateMemberRoleCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class UpdateMemberRoleCommandValidator : AbstractValidator<UpdateMemberRoleCommand>
{
    public UpdateMemberRoleCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("Team ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.NewRole)
            .IsInEnum()
            .WithMessage("Team role must be a valid value.");
    }
}
