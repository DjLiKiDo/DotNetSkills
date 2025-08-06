namespace DotNetSkills.Application.TaskExecution.Features.UpdateSubtask;

/// <summary>
/// Command to update an existing subtask.
/// </summary>
public record UpdateSubtaskCommand(
    TaskId TaskId,
    string Title,
    string? Description,
    string Priority,
    int? EstimatedHours,
    DateTime? DueDate,
    UserId UpdatedByUserId
) : IRequest<TaskResponse>;
