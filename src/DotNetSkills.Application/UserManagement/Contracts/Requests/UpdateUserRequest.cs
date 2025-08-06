using System.ComponentModel.DataAnnotations;

namespace DotNetSkills.Application.UserManagement.Contracts.Requests;

/// <summary>
/// Request DTO for updating an existing user.
/// This DTO contains the data that can be updated for a user account with proper validation attributes.
/// </summary>
public record UpdateUserRequest(
    [Required(ErrorMessage = "User name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "User name must be between 2 and 100 characters")]
    string Name,

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(254, ErrorMessage = "Email address cannot exceed 254 characters")]
    string Email)
{
    /// <summary>
    /// Converts this request to an UpdateUserCommand.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="updatedById">Optional ID of the user performing the update for authorization.</param>
    /// <returns>An UpdateUserCommand with the request data.</returns>
    public UpdateUserCommand ToCommand(UserId userId, UserId? updatedById = null)
    {
        return new UpdateUserCommand(userId, Name, Email, updatedById);
    }
}
