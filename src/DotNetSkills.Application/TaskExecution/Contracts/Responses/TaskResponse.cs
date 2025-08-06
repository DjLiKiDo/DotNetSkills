namespace DotNetSkills.Application.TaskExecution.Contracts.Responses;

/// <summary>
/// Response DTO for task information with calculated properties.
/// Contains comprehensive task details including progress tracking and timeline information.
/// </summary>
public record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    Guid ProjectId,
    Guid? AssignedUserId,
    string? AssignedUserName,
    Guid? ParentTaskId,
    string? ParentTaskTitle,
    int? EstimatedHours,
    int? ActualHours,
    DateTime? DueDate,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int SubtaskCount,
    decimal CompletionPercentage,
    TimeSpan? Duration,
    bool IsOverdue,
    bool IsAssigned,
    bool IsSubtask,
    bool HasSubtasks
)
{
    /// <summary>
    /// Indicates whether the task is currently active (not completed or cancelled).
    /// </summary>
    public bool IsActive => Status is not "Done" and not "Cancelled";

    /// <summary>
    /// Indicates whether the task can be modified.
    /// </summary>
    public bool CanBeModified => Status is not "Done";

    /// <summary>
    /// Gets the task urgency level based on due date and priority.
    /// </summary>
    public string UrgencyLevel
    {
        get
        {
            if (IsOverdue) return "Overdue";
            if (Priority == "Critical") return "Critical";
            if (DueDate.HasValue && DueDate.Value <= DateTime.UtcNow.AddDays(1)) return "Urgent";
            if (Priority == "High") return "High";
            return "Normal";
        }
    }
}
