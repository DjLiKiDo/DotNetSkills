using DotNetSkills.Application.TaskExecution.Projections;

namespace DotNetSkills.Application.TaskExecution.Contracts;

/// <summary>
/// Repository interface specific to Task entities.
/// Extends the generic repository with Task-specific query methods for task management operations.
/// </summary>
public interface ITaskRepository : IRepository<DotNetSkills.Domain.TaskExecution.Entities.Task, TaskId>
{
    /// <summary>
    /// Gets tasks by their associated project asynchronously.
    /// </summary>
    /// <param name="projectId">The project ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks belonging to the specified project.</returns>
    Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByProjectIdAsync(
        ProjectId projectId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks assigned to a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The user ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks assigned to the specified user.</returns>
    Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByAssigneeIdAsync(
        UserId userId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subtasks for a specific parent task asynchronously.
    /// </summary>
    /// <param name="parentTaskId">The parent task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of subtasks for the specified parent task.</returns>
    Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetSubtasksAsync(
        TaskId parentTaskId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a task with its subtasks included asynchronously.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task with subtasks if found, otherwise null.</returns>
    Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetWithSubtasksAsync(
        TaskId id, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks by their status asynchronously.
    /// </summary>
    /// <param name="status">The task status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks with the specified status.</returns>
    Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByStatusAsync(
        DotNetSkills.Domain.TaskExecution.Enums.TaskStatus status, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks by their priority asynchronously.
    /// </summary>
    /// <param name="priority">The task priority to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks with the specified priority.</returns>
    Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByPriorityAsync(
        TaskPriority priority, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets overdue tasks (past their due date).
    /// </summary>
    /// <param name="userId">Optional user filter to get overdue tasks for specific user.</param>
    /// <param name="projectId">Optional project filter to get overdue tasks for specific project.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of overdue tasks.</returns>
    Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetOverdueTasksAsync(
        UserId? userId = null,
        ProjectId? projectId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks with pagination support and comprehensive filtering asynchronously.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term to filter by title or description.</param>
    /// <param name="projectId">Optional project filter.</param>
    /// <param name="assignedUserId">Optional assigned user filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="priority">Optional priority filter.</param>
    /// <param name="dueDateFrom">Optional due date range filter (from).</param>
    /// <param name="dueDateTo">Optional due date range filter (to).</param>
    /// <param name="isOverdue">Optional filter for overdue tasks.</param>
    /// <param name="isSubtask">Optional filter to include/exclude subtasks.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated collection of tasks.</returns>
    Task<(IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> Tasks, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ProjectId? projectId = null,
        UserId? assignedUserId = null,
        DotNetSkills.Domain.TaskExecution.Enums.TaskStatus? status = null,
        TaskPriority? priority = null,
        DateTime? dueDateFrom = null,
        DateTime? dueDateTo = null,
        bool? isOverdue = null,
        bool? isSubtask = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task hierarchy for a specific task including all descendants.
    /// Returns the complete subtask tree structure.
    /// </summary>
    /// <param name="taskId">The root task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task with complete hierarchy of subtasks.</returns>
    Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetTaskHierarchyAsync(
        TaskId taskId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks that are approaching their deadline within the specified number of days.
    /// </summary>
    /// <param name="daysUntilDeadline">The number of days before deadline to consider.</param>
    /// <param name="userId">Optional user filter.</param>
    /// <param name="projectId">Optional project filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks approaching their deadline.</returns>
    Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetTasksApproachingDeadlineAsync(
        int daysUntilDeadline,
        UserId? userId = null,
        ProjectId? projectId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unassigned tasks that are available for assignment.
    /// </summary>
    /// <param name="projectId">Optional project filter.</param>
    /// <param name="priority">Optional priority filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of unassigned tasks.</returns>
    Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetUnassignedTasksAsync(
        ProjectId? projectId = null,
        TaskPriority? priority = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks with their time tracking information for reporting.
    /// </summary>
    /// <param name="projectId">Optional project filter.</param>
    /// <param name="userId">Optional user filter.</param>
    /// <param name="dateFrom">Optional start date filter.</param>
    /// <param name="dateTo">Optional end date filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks with time tracking data.</returns>
    Task<IEnumerable<(DotNetSkills.Domain.TaskExecution.Entities.Task Task, int? EstimatedHours, int? ActualHours)>> 
        GetTasksWithTimeTrackingAsync(
            ProjectId? projectId = null,
            UserId? userId = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a task has any subtasks.
    /// Used for validating task deletion and status changes.
    /// </summary>
    /// <param name="taskId">The task ID to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the task has subtasks, otherwise false.</returns>
    Task<bool> HasSubtasksAsync(TaskId taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks by their associated project as an async enumerable for streaming large result sets.
    /// Memory-efficient for processing many tasks in a project.
    /// </summary>
    /// <param name="projectId">The project ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of tasks belonging to the specified project.</returns>
    IAsyncEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> GetByProjectIdAsyncEnumerable(
        ProjectId projectId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks assigned to a specific user as an async enumerable.
    /// Memory-efficient for users with many assigned tasks.
    /// </summary>
    /// <param name="userId">The user ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of tasks assigned to the specified user.</returns>
    IAsyncEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> GetByAssigneeIdAsyncEnumerable(
        UserId userId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks by their status as an async enumerable for bulk operations.
    /// Optimized for memory efficiency when processing large numbers of tasks with specific status.
    /// </summary>
    /// <param name="status">The task status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of tasks with the specified status.</returns>
    IAsyncEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> GetByStatusAsyncEnumerable(
        DotNetSkills.Domain.TaskExecution.Enums.TaskStatus status, 
        CancellationToken cancellationToken = default);

    // Strategic Include Methods

    /// <summary>
    /// Gets a task with its project information included.
    /// Prevents N+1 queries when project details are needed.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task with project information if found, otherwise null.</returns>
    Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetWithProjectAsync(
        TaskId id, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks with their assigned user information included.
    /// Optimized for scenarios where user details are needed.
    /// </summary>
    /// <param name="projectId">The project ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks with assigned user information.</returns>
    Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetProjectTasksWithAssigneesAsync(
        ProjectId projectId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a task with complete hierarchy (parent and subtasks) included.
    /// Prevents multiple queries for task hierarchy operations.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task with complete hierarchy if found, otherwise null.</returns>
    Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetWithCompleteHierarchyAsync(
        TaskId id, 
        CancellationToken cancellationToken = default);

    // Projection Methods

    /// <summary>
    /// Gets task summaries with optimized projection for read-only scenarios.
    /// Minimizes data transfer by selecting only required fields.
    /// </summary>
    /// <param name="projectId">Optional project filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of task summary projections.</returns>
    Task<IEnumerable<TaskSummaryProjection>> GetTaskSummariesAsync(
        ProjectId? projectId = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task dashboard information with aggregated data.
    /// Optimized for dashboard scenarios with minimal queries.
    /// </summary>
    /// <param name="userId">Optional user filter for assigned tasks.</param>
    /// <param name="projectId">Optional project filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of task dashboard projections.</returns>
    Task<IEnumerable<TaskDashboardProjection>> GetTaskDashboardDataAsync(
        UserId? userId = null,
        ProjectId? projectId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task selection data for dropdowns and selection lists.
    /// Minimal projection for UI scenarios.
    /// </summary>
    /// <param name="projectId">Optional project filter.</param>
    /// <param name="assignableOnly">Whether to return only assignable tasks (unassigned or current user).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of task selection projections.</returns>
    Task<IEnumerable<TaskSelectionProjection>> GetTaskSelectionsAsync(
        ProjectId? projectId = null,
        bool assignableOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task assignment information for a specific user.
    /// Shows tasks assigned to the user with context.
    /// </summary>
    /// <param name="userId">The user ID to get task assignments for.</param>
    /// <param name="includeCompleted">Whether to include completed tasks.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of task assignment projections.</returns>
    Task<IEnumerable<TaskAssignmentProjection>> GetUserTaskAssignmentsAsync(
        UserId userId, 
        bool includeCompleted = false,
        CancellationToken cancellationToken = default);
}
