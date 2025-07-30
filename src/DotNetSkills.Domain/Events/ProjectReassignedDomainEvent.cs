using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record ProjectReassignedDomainEvent(
    ProjectId ProjectId,
    string ProjectName,
    TeamId PreviousTeamId,
    TeamId NewTeamId,
    UserId ReassignedBy,
    DateTime ReassignedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}