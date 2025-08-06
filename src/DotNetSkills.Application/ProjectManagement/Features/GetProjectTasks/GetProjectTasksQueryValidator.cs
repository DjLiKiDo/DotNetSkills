namespace DotNetSkills.Application.ProjectManagement.Features.GetProjectTasks;

/// <summary>
/// Validator for GetProjectTasksQuery that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class GetProjectTasksQueryValidator : AbstractValidator<GetProjectTasksQuery>
{
    private static readonly string[] ValidStatuses = { "ToDo", "InProgress", "InReview", "Done", "Cancelled" };
    private static readonly string[] ValidPriorities = { "Low", "Medium", "High", "Critical" };

    public GetProjectTasksQueryValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project ID is required.");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrEmpty(status) || ValidStatuses.Contains(status))
            .WithMessage("Status must be one of: ToDo, InProgress, InReview, Done, Cancelled.");

        RuleFor(x => x.Priority)
            .Must(priority => string.IsNullOrEmpty(priority) || ValidPriorities.Contains(priority))
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.");

        RuleFor(x => x.AssignedUserId)
            .NotEqual(new UserId(Guid.Empty))
            .When(x => x.AssignedUserId != null)
            .WithMessage("Assigned user ID cannot be empty GUID.");

        RuleFor(x => x.DueDateFrom)
            .LessThanOrEqualTo(x => x.DueDateTo)
            .When(x => x.DueDateFrom.HasValue && x.DueDateTo.HasValue)
            .WithMessage("Due date from cannot be greater than due date to.");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Search));
    }
}
