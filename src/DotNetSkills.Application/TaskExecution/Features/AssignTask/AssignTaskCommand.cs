namespace DotNetSkills.Application.TaskExecution.Features.AssignTask;

/// <summary>
/// Command to assign a task to a user.
/// </summary>
public record AssignTaskCommand(
    TaskId TaskId,
    UserId AssignedUserId,
    UserId AssignedByUserId
) : IRequest<TaskAssignmentResponse>;
