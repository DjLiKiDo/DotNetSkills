namespace DotNetSkills.Domain.UserManagement.Enums;

/// <summary>
/// Extension methods for UserRole enum providing business logic and utility methods.
/// </summary>
public static class UserRoleExtensions
{
    /// <summary>
    /// Gets the display name for the user role.
    /// </summary>
    /// <param name="role">The user role.</param>
    /// <returns>A human-readable display name.</returns>
    public static string GetDisplayName(this UserRole role) => role switch
    {
        UserRole.Viewer => "Viewer",
        UserRole.Developer => "Developer",
        UserRole.ProjectManager => "Project Manager",
        UserRole.Admin => "Administrator",
        _ => role.ToString()
    };

    /// <summary>
    /// Gets the permissions associated with the role (utility only).
    /// </summary>
    /// <param name="role">The user role.</param>
    /// <returns>A collection of permission strings.</returns>
    public static IReadOnlyList<string> GetPermissions(this UserRole role) => role switch
    {
        UserRole.Viewer => new[] { "read:projects", "read:tasks" },
        UserRole.Developer => new[] { "read:projects", "read:tasks", "update:tasks", "create:tasks" },
        UserRole.ProjectManager => new[] {
            "read:projects", "read:tasks", "update:tasks", "create:tasks",
            "create:projects", "update:projects", "manage:teams", "assign:tasks"
        },
        UserRole.Admin => new[] {
            "read:*", "create:*", "update:*", "delete:*",
            "manage:users", "manage:system"
        },
        _ => []
    };
}
