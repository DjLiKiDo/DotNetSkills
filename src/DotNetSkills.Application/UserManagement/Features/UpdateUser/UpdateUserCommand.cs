namespace DotNetSkills.Application.UserManagement.Features.UpdateUser;

/// <summary>
/// Command for updating user profile information (name and email).
/// Implements CQRS pattern with MediatR and Result pattern for error handling.
/// </summary>
/// <param name="UserId">The ID of the user to update.</param>
/// <param name="Name">The new name for the user.</param>
/// <param name="Email">The new email address for the user.</param>
/// <param name="UpdatedById">The ID of the user performing the update (optional for authorization).</param>
public record UpdateUserCommand(
    UserId UserId,
    string Name,
    string Email,
    UserId? UpdatedById = null) : IRequest<Result<UserResponse>>;
