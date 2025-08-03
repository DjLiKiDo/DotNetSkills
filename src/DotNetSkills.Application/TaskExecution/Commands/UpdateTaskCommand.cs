namespace DotNetSkills.Application.TaskExecution.Commands;

/// <summary>
/// Command for updating an existing task with comprehensive validation and business rule enforcement.
/// Updates task information while respecting domain constraints and status restrictions.
/// </summary>
public record UpdateTaskCommand(
    TaskId TaskId,
    string Title,
    string? Description,
    string Priority,
    int? EstimatedHours,
    DateTime? DueDate,
    UserId UpdatedBy
) : IRequest<TaskResponse>
{
    /// <summary>
    /// Validates the command parameters and throws ArgumentException if invalid.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when command parameters are invalid.</exception>
    public void Validate()
    {
        if (TaskId.Value == Guid.Empty)
            throw new ArgumentException("Task ID cannot be empty.", nameof(TaskId));

        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Task title is required and cannot be empty.", nameof(Title));

        if (Title.Length > 200)
            throw new ArgumentException("Task title cannot exceed 200 characters.", nameof(Title));

        if (Description?.Length > 2000)
            throw new ArgumentException("Task description cannot exceed 2000 characters.", nameof(Description));

        if (!IsValidPriority(Priority))
            throw new ArgumentException("Priority must be one of: Low, Medium, High, Critical.", nameof(Priority));

        if (EstimatedHours.HasValue && EstimatedHours <= 0)
            throw new ArgumentException("Estimated hours must be positive.", nameof(EstimatedHours));

        if (EstimatedHours.HasValue && EstimatedHours > 1000)
            throw new ArgumentException("Estimated hours cannot exceed 1000 hours.", nameof(EstimatedHours));

        if (DueDate.HasValue && DueDate <= DateTime.UtcNow)
            throw new ArgumentException("Due date must be in the future.", nameof(DueDate));

        if (UpdatedBy.Value == Guid.Empty)
            throw new ArgumentException("Updated by user ID cannot be empty.", nameof(UpdatedBy));
    }

    /// <summary>
    /// Validates task priority values.
    /// </summary>
    private static bool IsValidPriority(string priority)
    {
        return priority is "Low" or "Medium" or "High" or "Critical";
    }
}

/// <summary>
/// Placeholder handler for UpdateTaskCommand.
/// TODO: Replace with actual implementation when Infrastructure layer is available.
/// </summary>
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskResponse>
{
    public async Task<TaskResponse> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement actual command handling with repository
        // This would involve:
        // 1. Load existing task by ID
        // 2. Check if task exists and can be modified (not completed)
        // 3. Validate user has permission to update the task
        // 4. Parse priority to TaskPriority enum
        // 5. Update task using domain method (UpdateInfo)
        // 6. Save task through repository
        // 7. Map to response DTO and return

        await Task.CompletedTask;
        throw new NotImplementedException("UpdateTaskCommand requires Infrastructure layer implementation");
    }
}