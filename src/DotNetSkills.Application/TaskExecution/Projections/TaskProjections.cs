namespace DotNetSkills.Application.TaskExecution.Projections;

/// <summary>
/// Lightweight projection for task summary information.
/// Used for read-only scenarios to minimize data transfer.
/// </summary>
public record TaskSummaryProjection
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required DomainTaskStatus Status { get; init; }
    public required TaskPriority Priority { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? DueDate { get; init; }
    public required Guid ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public required Guid? AssignedUserId { get; init; }
    public required string? AssignedUserName { get; init; }
    public required int SubtaskCount { get; init; }
}

/// <summary>
/// Projection for task dashboard information.
/// Optimized for dashboard and overview scenarios.
/// </summary>
public record TaskDashboardProjection
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required DomainTaskStatus Status { get; init; }
    public required TaskPriority Priority { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required DateTime? DueDate { get; init; }
    public required Guid ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public required Guid? AssignedUserId { get; init; }
    public required string? AssignedUserName { get; init; }
    public required Guid? ParentTaskId { get; init; }
    public required string? ParentTaskTitle { get; init; }
    public required int SubtaskCount { get; init; }
    public required int CompletedSubtaskCount { get; init; }
    public required decimal CompletionPercentage { get; init; }
    public required bool IsOverdue { get; init; }
}

/// <summary>
/// Minimal projection for dropdown/selection scenarios.
/// Contains only essential identification information.
/// </summary>
public record TaskSelectionProjection
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required DomainTaskStatus Status { get; init; }
    public required TaskPriority Priority { get; init; }
    public required Guid ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public required bool CanHaveSubtasks { get; init; }
}

/// <summary>
/// Projection for task assignment scenarios.
/// Focused on assignee information and workload.
/// </summary>
public record TaskAssignmentProjection
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required DomainTaskStatus Status { get; init; }
    public required TaskPriority Priority { get; init; }
    public required DateTime? DueDate { get; init; }
    public required Guid ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public required Guid? AssignedUserId { get; init; }
    public required string? AssignedUserName { get; init; }
    public required string? AssignedUserEmail { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required int EstimatedHours { get; init; }
    public required bool IsOverdue { get; init; }
}
