using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record ValidationFailedDomainEvent(
    string EntityType,
    string EntityId,
    string OperationAttempted,
    string ValidationErrors,
    UserId? AttemptedBy,
    DateTime OccurredAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}