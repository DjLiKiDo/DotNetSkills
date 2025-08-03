namespace DotNetSkills.Application.ProjectManagement.Commands;

/// <summary>
/// Command for creating a new task within a specific project context.
/// Ensures the task belongs to the project and validates business rules.
/// </summary>
public record CreateTaskInProjectCommand(
    ProjectId ProjectId,
    string Title,
    string? Description,
    string Priority,
    TaskId? ParentTaskId,
    int? EstimatedHours,
    DateTime? DueDate,
    UserId? AssignedUserId,
    UserId CreatedBy
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

        if (ParentTaskId != null && ParentTaskId.Value == Guid.Empty)
            throw new ArgumentException("Parent task ID cannot be empty GUID.", nameof(ParentTaskId));

        if (AssignedUserId != null && AssignedUserId.Value == Guid.Empty)
            throw new ArgumentException("Assigned user ID cannot be empty GUID.", nameof(AssignedUserId));

        if (CreatedBy.Value == Guid.Empty)
            throw new ArgumentException("Created by user ID cannot be empty.", nameof(CreatedBy));
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
/// Placeholder handler for CreateTaskInProjectCommand.
/// TODO: Replace with actual implementation when Infrastructure layer is available.
/// </summary>
public class CreateTaskInProjectCommandHandler : IRequestHandler<CreateTaskInProjectCommand, ProjectTaskResponse>
{
    public async Task<ProjectTaskResponse> Handle(CreateTaskInProjectCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement actual command handling with repository
        // This would involve:
        // 1. Validate project exists and user has permission to create tasks in it
        // 2. Validate assigned user is a member of project's team (if specified)
        // 3. Validate parent task exists and belongs to same project (if specified)
        // 4. Create task using domain factory method
        // 5. Save task through repository
        // 6. Map to response DTO and return
        
        await Task.CompletedTask;
        throw new NotImplementedException("CreateTaskInProjectCommand requires Infrastructure layer implementation");
    }
}