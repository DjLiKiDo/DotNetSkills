namespace DotNetSkills.Application.UserManagement.Features.DeleteUser;

/// <summary>
/// Command to delete (soft delete) a user from the system.
/// This command represents a request to deactivate a user account.
/// </summary>
public record DeleteUserCommand(UserId UserId) : IRequest<UserResponse>;
