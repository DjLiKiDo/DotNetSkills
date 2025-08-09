namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeam;

/// <summary>
/// Validator for GetTeamByIdQuery that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class GetTeamByIdQueryValidator : AbstractValidator<GetTeamByIdQuery>
{
    public GetTeamByIdQueryValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("Team ID is required.");
    }
}
