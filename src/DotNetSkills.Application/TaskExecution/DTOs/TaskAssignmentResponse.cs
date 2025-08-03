namespace DotNetSkills.Application.TaskExecution.DTOs;

/// <summary>
/// Response DTO for task assignment operations.
/// </summary>
public record TaskAssignmentResponse(
    Guid TaskId,
    string TaskTitle,
    Guid? AssignedUserId,
    string? AssignedUserName,
    DateTime? AssignedAt,
    Guid AssignedByUserId,
    string AssignedByUserName
);

/// <summary>
/// Request DTO for assigning a task to a user.
/// </summary>
public record AssignTaskRequest(
    Guid UserId
)
{
    /// <summary>
    /// Validates the assign task request.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (UserId == Guid.Empty)
            throw new ArgumentException("User ID is required", nameof(UserId));
    }
};

/// <summary>
/// Response DTO for task subtasks listing.
/// </summary>
public record TaskSubtasksResponse(
    Guid ParentTaskId,
    string ParentTaskTitle,
    IReadOnlyList<SubtaskResponse> Subtasks,
    int TotalSubtasks,
    int CompletedSubtasks,
    decimal CompletionPercentage
);

/// <summary>
/// Response DTO for individual subtask information.
/// </summary>
public record SubtaskResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    Guid? AssignedUserId,
    string? AssignedUserName,
    int? EstimatedHours,
    int? ActualHours,
    DateTime? DueDate,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsOverdue,
    bool IsAssigned
);

/// <summary>
/// Request DTO for creating a subtask.
/// </summary>
public record CreateSubtaskRequest(
    string Title,
    string? Description,
    string Priority = "Medium",
    int? EstimatedHours = null,
    DateTime? DueDate = null,
    Guid? AssignedUserId = null
)
{
    /// <summary>
    /// Validates the create subtask request.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title is required", nameof(Title));

        if (Title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters", nameof(Title));

        if (Description is not null && Description.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters", nameof(Description));

        if (EstimatedHours.HasValue && EstimatedHours.Value <= 0)
            throw new ArgumentException("Estimated hours must be positive", nameof(EstimatedHours));

        if (DueDate.HasValue && DueDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("Due date must be in the future", nameof(DueDate));

        var validPriorities = new[] { "Low", "Medium", "High", "Critical" };
        if (!validPriorities.Contains(Priority))
            throw new ArgumentException($"Priority must be one of: {string.Join(", ", validPriorities)}", nameof(Priority));

        if (AssignedUserId.HasValue && AssignedUserId.Value == Guid.Empty)
            throw new ArgumentException("Assigned user ID cannot be empty GUID", nameof(AssignedUserId));
    }
};

/// <summary>
/// Request DTO for updating a subtask.
/// </summary>
public record UpdateSubtaskRequest(
    string Title,
    string? Description,
    string Priority,
    int? EstimatedHours = null,
    DateTime? DueDate = null
)
{
    /// <summary>
    /// Validates the update subtask request.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title is required", nameof(Title));

        if (Title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters", nameof(Title));

        if (Description is not null && Description.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters", nameof(Description));

        if (EstimatedHours.HasValue && EstimatedHours.Value <= 0)
            throw new ArgumentException("Estimated hours must be positive", nameof(EstimatedHours));

        if (DueDate.HasValue && DueDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("Due date must be in the future", nameof(DueDate));

        var validPriorities = new[] { "Low", "Medium", "High", "Critical" };
        if (!validPriorities.Contains(Priority))
            throw new ArgumentException($"Priority must be one of: {string.Join(", ", validPriorities)}", nameof(Priority));
    }
};
