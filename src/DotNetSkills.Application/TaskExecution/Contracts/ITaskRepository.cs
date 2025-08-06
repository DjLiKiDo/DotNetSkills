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
}
