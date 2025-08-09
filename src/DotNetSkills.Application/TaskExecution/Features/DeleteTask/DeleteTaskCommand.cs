namespace DotNetSkills.Application.TaskExecution.Features.DeleteTask;

/// <summary>
/// Command for deleting a task with proper authorization and business rule validation.
/// Supports soft delete by cancelling the task rather than hard deletion.
/// </summary>
public record DeleteTaskCommand(
    TaskId TaskId,
    UserId DeletedBy
) : IRequest;
