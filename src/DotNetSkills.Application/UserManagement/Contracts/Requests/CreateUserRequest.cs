using System.ComponentModel.DataAnnotations;

namespace DotNetSkills.Application.UserManagement.Contracts.Requests;

/// <summary>
/// Request DTO for creating a new user.
/// This DTO contains the data required to create a user account with proper validation attributes.
/// </summary>
public record CreateUserRequest(
    [Required(ErrorMessage = "User name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "User name must be between 2 and 100 characters")]
    string Name,

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(254, ErrorMessage = "Email address cannot exceed 254 characters")]
    string Email,

    [Required(ErrorMessage = "User role is required")]
    [RegularExpression("^(Admin|ProjectManager|Developer|Viewer)$", ErrorMessage = "Role must be Admin, ProjectManager, Developer, or Viewer")]
    string Role)
{
    /// <summary>
    /// Converts this request to a CreateUserCommand.
    /// </summary>
    /// <param name="createdById">Optional ID of the user performing the creation for authorization.</param>
    /// <returns>A CreateUserCommand with the request data.</returns>
    public CreateUserCommand ToCommand(UserId? createdById = null)
    {
        return new CreateUserCommand(Name, Email, Role, createdById);
    }
}
