namespace DotNetSkills.Application.TaskExecution.Features.GetTasks;

/// <summary>
/// Validator for GetTasksQuery with comprehensive validation rules.
/// </summary>
public class GetTasksQueryValidator : AbstractValidator<GetTasksQuery>
{
    private static readonly string[] ValidStatuses = { "ToDo", "InProgress", "InReview", "Done", "Cancelled" };
    private static readonly string[] ValidPriorities = { "Low", "Medium", "High", "Critical" };
    private static readonly string[] ValidSortFields = { "Title", "Status", "Priority", "DueDate", "CreatedAt", "UpdatedAt" };
    private static readonly string[] ValidSortDirections = { "asc", "desc" };

    public GetTasksQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be positive.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.ProjectId)
            .NotEqual(new ProjectId(Guid.Empty))
            .When(x => x.ProjectId != null)
            .WithMessage("Project ID cannot be empty GUID.");

        RuleFor(x => x.AssignedUserId)
            .NotEqual(new UserId(Guid.Empty))
            .When(x => x.AssignedUserId != null)
            .WithMessage("Assigned user ID cannot be empty GUID.");

        RuleFor(x => x.Status)
            .Must(status => ValidStatuses.Contains(status!))
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be one of: ToDo, InProgress, InReview, Done, Cancelled.");

        RuleFor(x => x.Priority)
            .Must(priority => ValidPriorities.Contains(priority!))
            .When(x => !string.IsNullOrEmpty(x.Priority))
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.");

        RuleFor(x => x.DueDateFrom)
            .LessThanOrEqualTo(x => x.DueDateTo)
            .When(x => x.DueDateFrom.HasValue && x.DueDateTo.HasValue)
            .WithMessage("Due date from cannot be greater than due date to.");

        RuleFor(x => x.CreatedFrom)
            .LessThanOrEqualTo(x => x.CreatedTo)
            .When(x => x.CreatedFrom.HasValue && x.CreatedTo.HasValue)
            .WithMessage("Created from cannot be greater than created to.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm))
            .WithMessage("Search term cannot exceed 100 characters.");

        RuleFor(x => x.SortBy)
            .Must(sortBy => ValidSortFields.Contains(sortBy))
            .WithMessage("Sort by must be one of: Title, Status, Priority, DueDate, CreatedAt, UpdatedAt.");

        RuleFor(x => x.SortDirection)
            .Must(direction => ValidSortDirections.Contains(direction.ToLowerInvariant()))
            .WithMessage("Sort direction must be 'asc' or 'desc'.");
    }
}
