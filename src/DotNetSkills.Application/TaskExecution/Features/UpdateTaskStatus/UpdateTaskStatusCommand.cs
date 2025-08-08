namespace DotNetSkills.Application.TaskExecution.Features.UpdateTaskStatus;

/// <summary>
/// Command for updating task status with proper state transition validation and business rule enforcement.
/// Supports all valid status transitions according to domain business rules.
/// </summary>
public record UpdateTaskStatusCommand(
    TaskId TaskId,
    DomainTaskStatus Status,
    int? ActualHours,
    UserId UpdatedBy
) : IRequest<TaskResponse>;
