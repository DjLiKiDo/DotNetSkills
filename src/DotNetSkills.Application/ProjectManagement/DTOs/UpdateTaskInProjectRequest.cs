namespace DotNetSkills.Application.ProjectManagement.DTOs;

/// <summary>
/// Request DTO for updating a task within a project context.
/// Contains updated task information while maintaining project relationship.
/// </summary>
public record UpdateTaskInProjectRequest
{
    /// <summary>
    /// The updated title of the task. Required and must not be empty.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Updated description providing more details about the task.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The updated priority level of the task.
    /// Valid values: Low, Medium, High, Critical
    /// </summary>
    public required string Priority { get; init; }

    /// <summary>
    /// Updated estimated effort in hours for completing the task.
    /// Must be positive if provided.
    /// </summary>
    public int? EstimatedHours { get; init; }

    /// <summary>
    /// Updated due date for the task completion.
    /// Must be in the future if provided (unless task is already completed).
    /// </summary>
    public DateTime? DueDate { get; init; }

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

        // Note: Due date validation against current time is handled in domain logic
        // since completed tasks can have past due dates

        if (errors.Count > 0)
            throw new ValidationException(string.Join(" ", errors));
    }

    /// <summary>
    /// Checks if the provided priority string is valid.
    /// </summary>
    private static bool IsValidPriority(string priority)
    {
        return priority is "Low" or "Medium" or "High" or "Critical";
    }
}