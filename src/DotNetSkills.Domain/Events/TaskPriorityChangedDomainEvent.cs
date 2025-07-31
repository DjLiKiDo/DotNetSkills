using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record TaskPriorityChangedDomainEvent(
    TaskId TaskId,
    string TaskTitle,
    ProjectId ProjectId,
    TaskPriority OldPriority,
    TaskPriority NewPriority,
    UserId ChangedBy,
    DateTime ChangedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}