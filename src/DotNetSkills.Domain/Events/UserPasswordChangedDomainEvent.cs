using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record UserPasswordChangedDomainEvent(
    UserId UserId,
    string FirstName,
    string LastName,
    EmailAddress Email,
    UserId ChangedBy,
    DateTime ChangedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTimeService.UtcNow;
}