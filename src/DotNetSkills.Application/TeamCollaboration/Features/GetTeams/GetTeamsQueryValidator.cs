namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeams;

/// <summary>
/// Validator for GetTeamsQuery that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class GetTeamsQueryValidator : AbstractValidator<GetTeamsQuery>
{
    public GetTeamsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Search));
    }
}
