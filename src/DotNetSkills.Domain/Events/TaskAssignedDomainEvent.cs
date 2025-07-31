using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record TaskAssignedDomainEvent(
    TaskId TaskId,
    string TaskTitle,
    ProjectId ProjectId,
    UserId AssignedToId,
    string AssignedToName,
    UserId? PreviousAssigneeId,
    string? PreviousAssigneeName,
    UserId AssignedBy,
    DateTime AssignedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTimeService.UtcNow;
}