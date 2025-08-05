namespace DotNetSkills.Application.TaskExecution.Queries;

/// <summary>
/// Query for retrieving tasks with comprehensive filtering and pagination.
/// Supports filtering by project, assignee, status, priority, due dates, and search terms.
/// </summary>
public record GetTasksQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? ProjectId = null,
    Guid? AssignedUserId = null,
    string? Status = null,
    string? Priority = null,
    DateTime? DueDateFrom = null,
    DateTime? DueDateTo = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    string? SearchTerm = null,
    bool IncludeSubtasks = true,
    bool OnlyOverdue = false,
    bool OnlyUnassigned = false,
    string SortBy = "CreatedAt",
    string SortDirection = "desc"
) : IRequest<PagedTaskResponse>
{
    /// <summary>
    /// Validates the query parameters and throws ArgumentException if invalid.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when query parameters are invalid.</exception>
    public void Validate()
    {
        if (PageNumber <= 0)
            throw new ArgumentException("Page number must be positive.", nameof(PageNumber));

        if (PageSize <= 0 || PageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100.", nameof(PageSize));

        if (ProjectId.HasValue && ProjectId == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty GUID.", nameof(ProjectId));

        if (AssignedUserId.HasValue && AssignedUserId == Guid.Empty)
            throw new ArgumentException("Assigned user ID cannot be empty GUID.", nameof(AssignedUserId));

        if (!string.IsNullOrEmpty(Status) && !IsValidStatus(Status))
            throw new ArgumentException("Status must be one of: ToDo, InProgress, InReview, Done, Cancelled.", nameof(Status));

        if (!string.IsNullOrEmpty(Priority) && !IsValidPriority(Priority))
            throw new ArgumentException("Priority must be one of: Low, Medium, High, Critical.", nameof(Priority));

        if (DueDateFrom.HasValue && DueDateTo.HasValue && DueDateFrom > DueDateTo)
            throw new ArgumentException("Due date from cannot be greater than due date to.", nameof(DueDateFrom));

        if (CreatedFrom.HasValue && CreatedTo.HasValue && CreatedFrom > CreatedTo)
            throw new ArgumentException("Created from cannot be greater than created to.", nameof(CreatedFrom));

        if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Length > 100)
            throw new ArgumentException("Search term cannot exceed 100 characters.", nameof(SearchTerm));

        if (!IsValidSortField(SortBy))
            throw new ArgumentException("Sort by must be one of: Title, Status, Priority, DueDate, CreatedAt, UpdatedAt.", nameof(SortBy));

        if (!IsValidSortDirection(SortDirection))
            throw new ArgumentException("Sort direction must be 'asc' or 'desc'.", nameof(SortDirection));
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

    /// <summary>
    /// Validates sort field values.
    /// </summary>
    private static bool IsValidSortField(string sortBy)
    {
        return sortBy is "Title" or "Status" or "Priority" or "DueDate" or "CreatedAt" or "UpdatedAt";
    }

    /// <summary>
    /// Validates sort direction values.
    /// </summary>
    private static bool IsValidSortDirection(string sortDirection)
    {
        return sortDirection.ToLowerInvariant() is "asc" or "desc";
    }
}

/// <summary>
/// Placeholder handler for GetTasksQuery.
/// TODO: Replace with actual implementation when Infrastructure layer is available.
/// </summary>
public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, PagedTaskResponse>
{
    public async Task<PagedTaskResponse> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement actual query handling with repository
        // This would involve:
        // 1. Apply filtering based on query parameters
        // 2. Apply sorting and pagination
        // 3. Load related data (assigned user, project, parent task)
        // 4. Calculate derived properties (IsOverdue, CompletionPercentage, etc.)
        // 5. Map to response DTOs
        // 6. Calculate aggregate statistics for filter metadata

        await Task.CompletedTask;
        throw new NotImplementedException("GetTasksQuery requires Infrastructure layer implementation");
    }
}
