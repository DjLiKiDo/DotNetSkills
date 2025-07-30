using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Events;

public sealed record UserRoleUpdatedDomainEvent(
    UserId UserId,
    string FirstName,
    string LastName,
    EmailAddress Email,
    UserRole OldRole,
    UserRole NewRole,
    UserId UpdatedBy,
    DateTime UpdatedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}