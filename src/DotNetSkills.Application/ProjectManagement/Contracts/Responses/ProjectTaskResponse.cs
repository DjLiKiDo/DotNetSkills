namespace DotNetSkills.Application.ProjectManagement.Contracts.Responses;

/// <summary>
/// Response DTO representing a task within a project context.
/// Contains task information with project-specific context and calculated properties.
/// </summary>
public record ProjectTaskResponse(
    Guid TaskId,
    string Title,
    string? Description,
    string Status,
    string Priority,
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
    public bool IsActive => Status != "Done" && Status != "Cancelled";

    /// <summary>
    /// Gets the task status in a human-readable format.
    /// </summary>
    public string StatusDisplay => Status switch
    {
        "ToDo" => "To Do",
        "InProgress" => "In Progress",
        "InReview" => "In Review",
        "Done" => "Done",
        "Cancelled" => "Cancelled",
        _ => Status
    };

    /// <summary>
    /// Gets the priority with color coding for UI display.
    /// </summary>
    public string PriorityColor => Priority switch
    {
        "Critical" => "red",
        "High" => "orange",
        "Medium" => "yellow",
        "Low" => "green",
        _ => "gray"
    };
}
