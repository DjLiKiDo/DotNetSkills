namespace DotNetSkills.Domain.TaskExecution.Events;

/// <summary>
/// Domain event raised when a task status changes.
/// </summary>
/// <param name="TaskId">The ID of the task whose status changed.</param>
/// <param name="PreviousStatus">The previous status of the task.</param>
/// <param name="NewStatus">The new status of the task.</param>
/// <param name="ChangedBy">The ID of the user who changed the status.</param>
public record TaskStatusChangedDomainEvent(
    TaskId TaskId,
    TaskStatus PreviousStatus,
    TaskStatus NewStatus,
    UserId ChangedBy) : BaseDomainEvent;
