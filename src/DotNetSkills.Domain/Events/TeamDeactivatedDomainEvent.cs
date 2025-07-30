using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record TeamDeactivatedDomainEvent(
    TeamId TeamId,
    string TeamName,
    string? Description,
    int MemberCount,
    UserId DeactivatedBy,
    DateTime DeactivatedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}