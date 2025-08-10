namespace DotNetSkills.Application.TaskExecution.Features.UnassignTask;

/// <summary>
/// Command to unassign a task from its current assignee.
/// </summary>
public record UnassignTaskCommand(
    TaskId TaskId,
    UserId UnassignedByUserId
) : IRequest<TaskAssignmentResponse>;
