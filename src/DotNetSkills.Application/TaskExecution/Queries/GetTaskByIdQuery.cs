namespace DotNetSkills.Application.TaskExecution.Queries;

/// <summary>
/// Query for retrieving a specific task by its ID with full details.
/// Returns complete task information including relationships and calculated properties.
/// </summary>
public record GetTaskByIdQuery(TaskId TaskId) : IRequest<TaskResponse?>
{
    /// <summary>
    /// Validates the query parameters and throws ArgumentException if invalid.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when query parameters are invalid.</exception>
    public void Validate()
    {
        if (TaskId.Value == Guid.Empty)
            throw new ArgumentException("Task ID cannot be empty.", nameof(TaskId));
    }
}

/// <summary>
/// Placeholder handler for GetTaskByIdQuery.
/// TODO: Replace with actual implementation when Infrastructure layer is available.
/// </summary>
public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskResponse?>
{
    public async Task<TaskResponse?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement actual query handling with repository
        // This would involve:
        // 1. Load task by ID with related data (assigned user, project, parent task, subtasks)
        // 2. Calculate derived properties (IsOverdue, CompletionPercentage, Duration, etc.)
        // 3. Map to response DTO
        // 4. Return null if task not found

        await Task.CompletedTask;
        throw new NotImplementedException("GetTaskByIdQuery requires Infrastructure layer implementation");
    }
}
