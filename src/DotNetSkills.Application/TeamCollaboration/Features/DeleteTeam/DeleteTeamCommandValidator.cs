namespace DotNetSkills.Application.TeamCollaboration.Features.DeleteTeam;

/// <summary>
/// Validator for DeleteTeamCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class DeleteTeamCommandValidator : AbstractValidator<DeleteTeamCommand>
{
    public DeleteTeamCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("Team ID is required.");
    }
}
