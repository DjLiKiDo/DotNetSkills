using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record UserActivatedDomainEvent(
    UserId UserId,
    string FirstName,
    string LastName,
    EmailAddress Email,
    UserRole Role,
    UserId ActivatedBy,
    DateTime ActivatedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}