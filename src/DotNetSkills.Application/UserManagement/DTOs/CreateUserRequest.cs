namespace DotNetSkills.Application.UserManagement.DTOs;

/// <summary>
/// Request DTO for creating a new user.
/// This DTO contains the data required to create a user account.
/// </summary>
public record CreateUserRequest(
    string Name,
    string Email,
    string Role)
{
    /// <summary>
    /// Converts this request to a CreateUserCommand.
    /// TODO: Replace with AutoMapper when properly configured.
    /// </summary>
    public Commands.CreateUserCommand ToCommand()
    {
        return new Commands.CreateUserCommand(Name, Email, Role);
    }
}
