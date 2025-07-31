using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record TaskStatusUpdatedDomainEvent(
    TaskId TaskId,
    string TaskTitle,
    ProjectId ProjectId,
    Enums.TaskStatus PreviousStatus,
    Enums.TaskStatus NewStatus,
    UserId? AssignedToId,
    UserId UpdatedBy,
    DateTime UpdatedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTimeService.UtcNow;
}