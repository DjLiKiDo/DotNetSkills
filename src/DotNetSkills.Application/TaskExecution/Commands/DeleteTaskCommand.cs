namespace DotNetSkills.Application.TaskExecution.Commands;

/// <summary>
/// Command for deleting a task with proper authorization and business rule validation.
/// Supports soft delete by cancelling the task rather than hard deletion.
/// </summary>
public record DeleteTaskCommand(
    TaskId TaskId,
    UserId DeletedBy
) : IRequest
{
    /// <summary>
    /// Validates the command parameters and throws ArgumentException if invalid.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when command parameters are invalid.</exception>
    public void Validate()
    {
        if (TaskId.Value == Guid.Empty)
            throw new ArgumentException("Task ID cannot be empty.", nameof(TaskId));

        if (DeletedBy.Value == Guid.Empty)
            throw new ArgumentException("Deleted by user ID cannot be empty.", nameof(DeletedBy));
    }
}

/// <summary>
/// Placeholder handler for DeleteTaskCommand.
/// TODO: Replace with actual implementation when Infrastructure layer is available.
/// </summary>
public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement actual command handling with repository
        // This would involve:
        // 1. Load existing task by ID
        // 2. Check if task exists and can be deleted (not completed)
        // 3. Validate user has permission to delete the task
        // 4. Cancel task using domain method (Cancel) - this is soft delete
        // 5. This will also cancel all subtasks automatically
        // 6. Save task through repository

        await Task.CompletedTask;
        throw new NotImplementedException("DeleteTaskCommand requires Infrastructure layer implementation");
    }
}