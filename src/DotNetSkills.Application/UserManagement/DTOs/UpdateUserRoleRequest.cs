using System.ComponentModel.DataAnnotations;

namespace DotNetSkills.Application.UserManagement.DTOs;

/// <summary>
/// Request DTO for updating a user's role.
/// This DTO contains the new role to assign to a user with proper validation attributes.
/// </summary>
public record UpdateUserRoleRequest(
    [Required(ErrorMessage = "User role is required")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "Role must be a valid UserRole (Admin, ProjectManager, Developer, or Viewer)")]
    UserRole Role)
{
    /// <summary>
    /// Converts this request to an UpdateUserRoleCommand.
    /// </summary>
    /// <param name="userId">The ID of the user whose role is being changed.</param>
    /// <param name="changedById">The ID of the user performing the role change (required for authorization).</param>
    /// <returns>An UpdateUserRoleCommand with the request data.</returns>
    public Commands.UpdateUserRoleCommand ToCommand(UserId userId, UserId changedById)
    {
        return new Commands.UpdateUserRoleCommand(userId, Role, changedById);
    }
}
