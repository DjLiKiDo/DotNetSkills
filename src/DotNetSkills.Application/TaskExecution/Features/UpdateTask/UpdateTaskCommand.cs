namespace DotNetSkills.Application.TaskExecution.Features.UpdateTask;

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
) : IRequest<TaskResponse>;
