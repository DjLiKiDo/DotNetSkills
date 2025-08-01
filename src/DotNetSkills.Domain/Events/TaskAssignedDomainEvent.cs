namespace DotNetSkills.Domain.Events;

/// <summary>
/// Domain event raised when a task is assigned to a user.
/// </summary>
/// <param name="TaskId">The ID of the assigned task.</param>
/// <param name="AssigneeId">The ID of the user assigned to the task.</param>
/// <param name="AssignedBy">The ID of the user who made the assignment.</param>
public record TaskAssignedDomainEvent(
    TaskId TaskId,
    UserId AssigneeId,
    UserId AssignedBy) : BaseDomainEvent;
