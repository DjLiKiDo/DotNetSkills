using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record TaskUnassignedDomainEvent(
    TaskId TaskId,
    string TaskTitle,
    ProjectId ProjectId,
    UserId PreviousAssigneeId,
    string PreviousAssigneeName,
    UserId UnassignedBy,
    DateTime UnassignedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}