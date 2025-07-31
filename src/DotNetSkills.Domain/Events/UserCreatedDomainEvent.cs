using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record UserCreatedDomainEvent(
    UserId UserId,
    string FirstName,
    string LastName,
    EmailAddress Email,
    UserRole Role,
    DateTime CreatedAt,
    UserId CreatedBy
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTimeService.UtcNow;
}