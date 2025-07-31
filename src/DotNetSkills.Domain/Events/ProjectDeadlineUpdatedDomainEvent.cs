using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record ProjectDeadlineUpdatedDomainEvent(
    ProjectId ProjectId,
    string ProjectName,
    TeamId TeamId,
    DateTime? OldEndDate,
    DateTime? NewEndDate,
    UserId ChangedBy,
    DateTime ChangedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}