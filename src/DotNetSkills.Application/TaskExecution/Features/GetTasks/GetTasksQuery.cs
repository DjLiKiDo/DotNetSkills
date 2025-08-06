namespace DotNetSkills.Application.TaskExecution.Features.GetTasks;

/// <summary>
/// Query for retrieving tasks with comprehensive filtering and pagination.
/// Supports filtering by project, assignee, status, priority, due dates, and search terms.
/// </summary>
public record GetTasksQuery(
    int PageNumber = 1,
    int PageSize = 20,
    ProjectId? ProjectId = null,
    UserId? AssignedUserId = null,
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
) : IRequest<PagedTaskResponse>;
