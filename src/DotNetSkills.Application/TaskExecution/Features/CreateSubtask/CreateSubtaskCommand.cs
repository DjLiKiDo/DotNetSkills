namespace DotNetSkills.Application.TaskExecution.Features.CreateSubtask;

/// <summary>
/// Command to create a subtask under a parent task.
/// </summary>
public record CreateSubtaskCommand(
    TaskId ParentTaskId,
    string Title,
    string? Description,
    TaskPriority Priority,
    int? EstimatedHours,
    DateTime? DueDate,
    UserId? AssignedUserId,
    UserId CreatedByUserId
) : IRequest<TaskResponse>;
