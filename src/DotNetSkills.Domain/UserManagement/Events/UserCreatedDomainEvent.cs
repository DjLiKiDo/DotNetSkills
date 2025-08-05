namespace DotNetSkills.Domain.UserManagement.Events;

/// <summary>
/// Domain event raised when a new user is created in the system.
/// </summary>
/// <param name="UserId">The ID of the created user.</param>
/// <param name="Email">The email address of the created user.</param>
/// <param name="Name">The name of the created user.</param>
/// <param name="Role">The role assigned to the created user.</param>
/// <param name="CreatedBy">The ID of the user who created this user (typically an admin).</param>
public record UserCreatedDomainEvent(
    UserId UserId,
    EmailAddress Email,
    string Name,
    UserRole Role,
    UserId? CreatedBy) : BaseDomainEvent;
