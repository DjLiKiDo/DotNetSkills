using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record TaskCompletedDomainEvent(
    TaskId TaskId,
    string TaskTitle,
    ProjectId ProjectId,
    UserId? AssignedToId,
    string? AssignedToName,
    TaskPriority Priority,
    decimal? EstimatedHours,
    decimal? ActualHours,
    DateTime CompletedAt,
    UserId CompletedBy
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTimeService.UtcNow;
}