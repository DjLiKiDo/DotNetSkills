using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record ProjectStatusUpdatedDomainEvent(
    ProjectId ProjectId,
    string ProjectName,
    TeamId TeamId,
    ProjectStatus PreviousStatus,
    ProjectStatus NewStatus,
    UserId UpdatedBy,
    DateTime UpdatedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}