namespace DotNetSkills.Domain.TaskExecution.Services;

/// <summary>
/// Domain service for complex task management operations that require external dependencies
/// or cross-aggregate coordination. This service handles task-related business logic that
/// involves multiple aggregates or requires repository access.
/// </summary>
/// <remarks>
/// This interface defines contracts for operations that:
/// - Require database access to check task dependencies
/// - Involve cross-aggregate business logic (tasks, users, projects)
/// - Orchestrate complex task assignment and management operations
///
/// Simple task business rules remain in BusinessRules.TaskStatus for performance.
/// </remarks>
public interface ITaskDomainService
{
    /// <summary>
    /// Validates if a task can be safely deleted from the system.
    /// This involves checking for subtasks, dependencies, and completion status.
    /// </summary>
    /// <param name="taskId">The ID of the task to validate for deletion.</param>
    /// <param name="requestingUser">The user requesting the deletion (for authorization).</param>
    /// <returns>True if the task can be deleted, false otherwise.</returns>
    Task<bool> CanDeleteTaskAsync(TaskId taskId, User requestingUser);

    /// <summary>
    /// Validates if a task can be assigned to a specific user considering workload,
    /// permissions, and business rules.
    /// </summary>
    /// <param name="taskId">The ID of the task to assign.</param>
    /// <param name="assigneeId">The ID of the user to assign the task to.</param>
    /// <param name="assignedByUser">The user making the assignment.</param>
    /// <returns>True if the task can be assigned, false otherwise.</returns>
    Task<bool> CanAssignTaskAsync(TaskId taskId, UserId assigneeId, User assignedByUser);

    /// <summary>
    /// Validates if a task can transition to a new status considering current state,
    /// subtasks, and business dependencies.
    /// </summary>
    /// <param name="taskId">The ID of the task.</param>
    /// <param name="newStatus">The target status for the transition.</param>
    /// <param name="requestingUser">The user requesting the status change.</param>
    /// <returns>True if the status transition is allowed, false otherwise.</returns>
    Task<bool> CanTransitionToStatusAsync(TaskId taskId, TaskStatus newStatus, User requestingUser);

    /// <summary>
    /// Checks if a user has exceeded their maximum concurrent task assignments.
    /// </summary>
    /// <param name="userId">The ID of the user to check.</param>
    /// <returns>True if the user has reached their assignment limit, false otherwise.</returns>
    Task<bool> HasReachedAssignmentLimitAsync(UserId userId);

    /// <summary>
    /// Validates if a task can have subtasks created considering current status
    /// and business rules.
    /// </summary>
    /// <param name="parentTaskId">The ID of the parent task.</param>
    /// <param name="requestingUser">The user requesting subtask creation.</param>
    /// <returns>True if subtasks can be created, false otherwise.</returns>
    Task<bool> CanCreateSubtasksAsync(TaskId parentTaskId, User requestingUser);

    /// <summary>
    /// Calculates comprehensive task metrics including progress, time tracking,
    /// and subtask analysis.
    /// </summary>
    /// <param name="taskId">The ID of the task to analyze.</param>
    /// <returns>The task metrics analysis result.</returns>
    Task<TaskMetricsResult> CalculateTaskMetricsAsync(TaskId taskId);

    /// <summary>
    /// Validates task dependencies and identifies potential circular references
    /// or invalid dependency chains.
    /// </summary>
    /// <param name="taskId">The ID of the task to validate.</param>
    /// <param name="dependencyTaskIds">The IDs of tasks that this task depends on.</param>
    /// <returns>True if the dependencies are valid, false if there are conflicts.</returns>
    Task<bool> ValidateTaskDependenciesAsync(TaskId taskId, IEnumerable<TaskId> dependencyTaskIds);
}

/// <summary>
/// Result object for task metrics analysis.
/// </summary>
public class TaskMetricsResult
{
    /// <summary>
    /// Gets the current task status.
    /// </summary>
    public TaskStatus CurrentStatus { get; init; }

    /// <summary>
    /// Gets the number of subtasks (if any).
    /// </summary>
    public int SubtaskCount { get; init; }

    /// <summary>
    /// Gets the number of completed subtasks.
    /// </summary>
    public int CompletedSubtasks { get; init; }

    /// <summary>
    /// Gets the estimated hours for the task.
    /// </summary>
    public int? EstimatedHours { get; init; }

    /// <summary>
    /// Gets the actual hours spent on the task.
    /// </summary>
    public int? ActualHours { get; init; }

    /// <summary>
    /// Gets the task completion percentage (0-100).
    /// </summary>
    public double CompletionPercentage { get; init; }

    /// <summary>
    /// Gets whether the task is overdue.
    /// </summary>
    public bool IsOverdue { get; init; }

    /// <summary>
    /// Gets the number of days until the due date (negative if overdue).
    /// </summary>
    public int? DaysUntilDue { get; init; }

    /// <summary>
    /// Gets whether the task is on track for completion.
    /// </summary>
    public bool IsOnTrack { get; init; }

    /// <summary>
    /// Gets the user ID of the assignee (if assigned).
    /// </summary>
    public UserId? AssigneeId { get; init; }

    /// <summary>
    /// Gets the priority level of the task.
    /// </summary>
    public TaskPriority Priority { get; init; }

    /// <summary>
    /// Gets additional notes about the task metrics.
    /// </summary>
    public string? Notes { get; init; }
}
