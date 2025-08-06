namespace DotNetSkills.Application.TeamCollaboration.Features.AddTeamMember;

/// <summary>
/// Validator for AddTeamMemberCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class AddTeamMemberCommandValidator : AbstractValidator<AddTeamMemberCommand>
{
    public AddTeamMemberCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("Team ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Team role must be a valid value.");
    }
}
