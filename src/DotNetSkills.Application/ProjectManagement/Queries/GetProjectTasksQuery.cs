namespace DotNetSkills.Application.ProjectManagement.Queries;

/// <summary>
/// Query for retrieving tasks that belong to a specific project with filtering and pagination.
/// Supports comprehensive filtering by status, assignee, priority, due dates, and search terms.
/// </summary>
public record GetProjectTasksQuery(
    ProjectId ProjectId,
    int Page = 1,
    int PageSize = 20,
    string? Status = null,
    Guid? AssignedUserId = null,
    string? Priority = null,
    DateTime? DueDateFrom = null,
    DateTime? DueDateTo = null,
    bool? IsOverdue = null,
    bool? IsSubtask = null,
    string? Search = null
) : IRequest<PagedProjectTaskResponse>
{
    /// <summary>
    /// Validates the query parameters and throws ArgumentException if invalid.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when query parameters are invalid.</exception>
    public void Validate()
    {
        if (ProjectId.Value == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty.", nameof(ProjectId));

        if (Page < 1)
            throw new ArgumentException("Page number must be greater than 0.", nameof(Page));

        if (PageSize < 1 || PageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100.", nameof(PageSize));

        if (!string.IsNullOrEmpty(Status) && !IsValidStatus(Status))
            throw new ArgumentException("Status must be one of: ToDo, InProgress, InReview, Done, Cancelled.", nameof(Status));

        if (!string.IsNullOrEmpty(Priority) && !IsValidPriority(Priority))
            throw new ArgumentException("Priority must be one of: Low, Medium, High, Critical.", nameof(Priority));

        if (AssignedUserId.HasValue && AssignedUserId == Guid.Empty)
            throw new ArgumentException("Assigned user ID cannot be empty GUID.", nameof(AssignedUserId));

        if (DueDateFrom.HasValue && DueDateTo.HasValue && DueDateFrom > DueDateTo)
            throw new ArgumentException("Due date from cannot be greater than due date to.", nameof(DueDateFrom));

        if (!string.IsNullOrEmpty(Search) && Search.Length > 100)
            throw new ArgumentException("Search term cannot exceed 100 characters.", nameof(Search));
    }

    /// <summary>
    /// Validates task status values.
    /// </summary>
    private static bool IsValidStatus(string status)
    {
        return status is "ToDo" or "InProgress" or "InReview" or "Done" or "Cancelled";
    }

    /// <summary>
    /// Validates task priority values.
    /// </summary>
    private static bool IsValidPriority(string priority)
    {
        return priority is "Low" or "Medium" or "High" or "Critical";
    }
}

/// <summary>
/// Placeholder handler for GetProjectTasksQuery.
/// TODO: Replace with actual implementation when Infrastructure layer is available.
/// </summary>
public class GetProjectTasksQueryHandler : IRequestHandler<GetProjectTasksQuery, PagedProjectTaskResponse>
{
    public async Task<PagedProjectTaskResponse> Handle(GetProjectTasksQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement actual query handling with repository
        // This is a placeholder implementation
        await Task.CompletedTask;
        
        return new PagedProjectTaskResponse(
            Tasks: new List<ProjectTaskResponse>(),
            TotalCount: 0,
            Page: request.Page,
            PageSize: request.PageSize,
            ProjectId: request.ProjectId.Value,
            ProjectName: "Sample Project", // TODO: Get from repository
            ActiveTaskCount: 0,
            CompletedTaskCount: 0,
            OverdueTaskCount: 0
        );
    }
}