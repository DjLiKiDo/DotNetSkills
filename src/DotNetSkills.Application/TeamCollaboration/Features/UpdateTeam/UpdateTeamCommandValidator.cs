namespace DotNetSkills.Application.TeamCollaboration.Features.UpdateTeam;

/// <summary>
/// Validator for UpdateTeamCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand>
{
    public UpdateTeamCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("Team ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Team name is required.")
            .MaximumLength(100)
            .WithMessage("Team name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Team description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
