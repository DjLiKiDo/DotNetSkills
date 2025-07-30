using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record UserDeactivatedDomainEvent(
    UserId UserId,
    string FirstName,
    string LastName,
    EmailAddress Email,
    UserRole Role,
    UserId DeactivatedBy,
    DateTime DeactivatedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}