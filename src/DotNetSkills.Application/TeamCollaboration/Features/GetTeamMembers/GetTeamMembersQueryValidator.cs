namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeamMembers;

/// <summary>
/// Validator for GetTeamMembersQuery that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class GetTeamMembersQueryValidator : AbstractValidator<GetTeamMembersQuery>
{
    public GetTeamMembersQueryValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("Team ID is required.");
    }
}
