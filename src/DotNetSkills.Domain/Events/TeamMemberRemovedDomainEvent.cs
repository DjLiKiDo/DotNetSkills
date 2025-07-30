using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record TeamMemberRemovedDomainEvent(
    TeamId TeamId,
    string TeamName,
    UserId UserId,
    string UserName,
    string UserEmail,
    UserId RemovedBy,
    DateTime RemovedAt,
    TimeSpan MembershipDuration
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}