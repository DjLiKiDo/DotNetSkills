using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record TeamActivatedDomainEvent(
    TeamId TeamId,
    string TeamName,
    string? Description,
    int MemberCount,
    UserId ActivatedBy,
    DateTime ActivatedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}