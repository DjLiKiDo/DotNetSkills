namespace DotNetSkills.Application.ProjectManagement.Features.GetProjects;

/// <summary>
/// Validator for GetProjectsQuery that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class GetProjectsQueryValidator : AbstractValidator<GetProjectsQuery>
{
    public GetProjectsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.StartDateFrom)
            .LessThanOrEqualTo(x => x.StartDateTo)
            .When(x => x.StartDateFrom.HasValue && x.StartDateTo.HasValue)
            .WithMessage("Start date from cannot be greater than start date to.");

        RuleFor(x => x.EndDateFrom)
            .LessThanOrEqualTo(x => x.EndDateTo)
            .When(x => x.EndDateFrom.HasValue && x.EndDateTo.HasValue)
            .WithMessage("End date from cannot be greater than end date to.");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Search));
    }
}
