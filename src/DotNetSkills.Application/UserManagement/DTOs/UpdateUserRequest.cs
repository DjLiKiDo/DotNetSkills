namespace DotNetSkills.Application.UserManagement.DTOs;

/// <summary>
/// Request DTO for updating an existing user.
/// This DTO contains the data that can be updated for a user account.
/// </summary>
public record UpdateUserRequest(
    string Name,
    string Email)
{
    /// <summary>
    /// Converts this request to an UpdateUserCommand.
    /// TODO: Replace with AutoMapper when properly configured.
    /// </summary>
    public Commands.UpdateUserCommand ToCommand(Guid userId)
    {
        return new Commands.UpdateUserCommand(userId, Name, Email);
    }
}
