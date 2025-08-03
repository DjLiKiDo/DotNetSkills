namespace DotNetSkills.Application.ProjectManagement.Commands;

/// <summary>
/// Command for updating a task within a specific project context.
/// Ensures the task belongs to the project and validates business rules.
/// </summary>
public record UpdateTaskInProjectCommand(
    ProjectId ProjectId,
    TaskId TaskId,
    string Title,
    string? Description,
    string Priority,
    int? EstimatedHours,
    DateTime? DueDate,
    UserId UpdatedBy
) : IRequest<ProjectTaskResponse>
{
    /// <summary>
    /// Validates the command parameters and throws ArgumentException if invalid.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when command parameters are invalid.</exception>
    public void Validate()
    {
        if (ProjectId.Value == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty.", nameof(ProjectId));

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

        if (UpdatedBy.Value == Guid.Empty)
            throw new ArgumentException("Updated by user ID cannot be empty.", nameof(UpdatedBy));

        // Note: Due date validation against current time is handled in domain logic
        // since completed tasks can have past due dates
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
/// Placeholder handler for UpdateTaskInProjectCommand.
/// TODO: Replace with actual implementation when Infrastructure layer is available.
/// </summary>
public class UpdateTaskInProjectCommandHandler : IRequestHandler<UpdateTaskInProjectCommand, ProjectTaskResponse>
{
    public async Task<ProjectTaskResponse> Handle(UpdateTaskInProjectCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement actual command handling with repository
        // This would involve:
        // 1. Validate project exists and user has permission to modify tasks in it
        // 2. Validate task exists and belongs to the specified project
        // 3. Validate task is not completed (domain rule)
        // 4. Update task using domain methods
        // 5. Save task through repository
        // 6. Map to response DTO and return
        
        await Task.CompletedTask;
        throw new NotImplementedException("UpdateTaskInProjectCommand requires Infrastructure layer implementation");
    }
}