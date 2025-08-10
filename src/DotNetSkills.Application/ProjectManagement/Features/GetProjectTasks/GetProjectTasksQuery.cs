namespace DotNetSkills.Application.ProjectManagement.Features.GetProjectTasks;

/// <summary>
/// Query for retrieving tasks that belong to a specific project with filtering and pagination.
/// Supports comprehensive filtering by status, assignee, priority, due dates, and search terms.
/// </summary>
public record GetProjectTasksQuery(
    ProjectId ProjectId,
    int Page = 1,
    int PageSize = 20,
    DomainTaskStatus? Status = null,
    UserId? AssignedUserId = null,
    TaskPriority? Priority = null,
    DateTime? DueDateFrom = null,
    DateTime? DueDateTo = null,
    bool? IsOverdue = null,
    bool? IsSubtask = null,
    string? Search = null
) : IRequest<PagedProjectTaskResponse>;
