namespace DotNetSkills.Application.ProjectManagement.DTOs;

/// <summary>
/// Request DTO for creating a new task within a project context.
/// Contains all necessary information to create a task that belongs to a specific project.
/// </summary>
public record CreateTaskInProjectRequest
{
    /// <summary>
    /// The title of the task. Required and must not be empty.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Optional description providing more details about the task.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The priority level of the task.
    /// Valid values: Low, Medium, High, Critical
    /// </summary>
    public required string Priority { get; init; }

    /// <summary>
    /// Optional ID of the parent task if this is a subtask.
    /// Must belong to the same project and cannot be a subtask itself (single-level nesting only).
    /// </summary>
    public Guid? ParentTaskId { get; init; }

    /// <summary>
    /// Optional estimated effort in hours for completing the task.
    /// Must be positive if provided.
    /// </summary>
    public int? EstimatedHours { get; init; }

    /// <summary>
    /// Optional due date for the task completion.
    /// Must be in the future if provided.
    /// </summary>
    public DateTime? DueDate { get; init; }

    /// <summary>
    /// Optional user ID to assign the task to immediately upon creation.
    /// User must be a member of the project's team.
    /// </summary>
    public Guid? AssignedUserId { get; init; }

    /// <summary>
    /// Validates the request data and throws ValidationException if invalid.
    /// </summary>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    public void Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Title))
            errors.Add("Task title is required and cannot be empty.");

        if (Title?.Length > 200)
            errors.Add("Task title cannot exceed 200 characters.");

        if (Description?.Length > 2000)
            errors.Add("Task description cannot exceed 2000 characters.");

        if (!IsValidPriority(Priority))
            errors.Add("Priority must be one of: Low, Medium, High, Critical.");

        if (EstimatedHours.HasValue && EstimatedHours <= 0)
            errors.Add("Estimated hours must be positive.");

        if (EstimatedHours.HasValue && EstimatedHours > 1000)
            errors.Add("Estimated hours cannot exceed 1000 hours.");

        if (DueDate.HasValue && DueDate <= DateTime.UtcNow)
            errors.Add("Due date must be in the future.");

        if (ParentTaskId.HasValue && ParentTaskId == Guid.Empty)
            errors.Add("Parent task ID cannot be empty GUID.");

        if (AssignedUserId.HasValue && AssignedUserId == Guid.Empty)
            errors.Add("Assigned user ID cannot be empty GUID.");

        if (errors.Count > 0)
            throw new System.ComponentModel.DataAnnotations.ValidationException(string.Join(" ", errors));
    }

    /// <summary>
    /// Checks if the provided priority string is valid.
    /// </summary>
    private static bool IsValidPriority(string priority)
    {
        return priority is "Low" or "Medium" or "High" or "Critical";
    }
}
