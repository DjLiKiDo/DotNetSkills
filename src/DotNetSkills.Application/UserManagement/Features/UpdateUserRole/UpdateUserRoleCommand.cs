namespace DotNetSkills.Application.UserManagement.Features.UpdateUserRole;

/// <summary>
/// Command for updating a user's role in the system.
/// Implements CQRS pattern with MediatR and Result pattern for error handling.
/// </summary>
/// <param name="UserId">The ID of the user whose role is being changed.</param>
/// <param name="Role">The new role to assign to the user.</param>
/// <param name="ChangedById">The ID of the user performing the role change (required for authorization).</param>
public record UpdateUserRoleCommand(
    UserId UserId,
    UserRole Role,
    UserId ChangedById) : IRequest<UserResponse>;
