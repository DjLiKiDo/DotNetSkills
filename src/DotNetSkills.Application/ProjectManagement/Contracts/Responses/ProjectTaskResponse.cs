namespace DotNetSkills.Application.ProjectManagement.Contracts.Responses;

/// <summary>
/// Response DTO representing a task within a project context.
/// Contains task information with project-specific context and calculated properties.
/// </summary>
public record ProjectTaskResponse(
    Guid TaskId,
    string Title,
    string? Description,
    DomainTaskStatus Status,
    TaskPriority Priority,
    Guid ProjectId,
    Guid? AssignedUserId,
    string? AssignedUserName,
    Guid? ParentTaskId,
    int? EstimatedHours,
    int? ActualHours,
    DateTime? DueDate,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsOverdue,
    bool IsAssigned,
    bool IsSubtask,
    bool HasSubtasks,
    decimal CompletionPercentage,
    TimeSpan? Duration,
    int SubtaskCount
)
{
    /// <summary>
    /// Indicates whether the task is currently active (not completed or cancelled).
    /// </summary>
    public bool IsActive => Status is not DomainTaskStatus.Done and not DomainTaskStatus.Cancelled;

    /// <summary>
    /// Gets the task status in a human-readable format.
    /// </summary>
    public string StatusDisplay => Status switch
    {
        DomainTaskStatus.ToDo => "To Do",
        DomainTaskStatus.InProgress => "In Progress",
        DomainTaskStatus.InReview => "In Review",
        DomainTaskStatus.Done => "Done",
        DomainTaskStatus.Cancelled => "Cancelled",
        _ => Status.ToString()
    };

    /// <summary>
    /// Gets the priority with color coding for UI display.
    /// </summary>
    public string PriorityColor => Priority switch
    {
        TaskPriority.Critical => "red",
        TaskPriority.High => "orange",
        TaskPriority.Medium => "yellow",
        TaskPriority.Low => "green",
        _ => "gray"
    };
}
