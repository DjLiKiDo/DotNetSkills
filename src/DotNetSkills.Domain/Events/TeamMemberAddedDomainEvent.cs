using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record TeamMemberAddedDomainEvent(
    TeamId TeamId,
    string TeamName,
    UserId UserId,
    string UserName,
    string UserEmail,
    UserId AddedBy,
    DateTime JoinedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}