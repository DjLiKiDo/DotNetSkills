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
    /// Checks if the role has permission to create users.
    /// </summary>
    /// <param name="role">The user role.</param>
    /// <returns>True if the role can create users, false otherwise.</returns>
    public static bool CanCreateUsers(this UserRole role) =>
        role == UserRole.Admin;

    /// <summary>
    /// Checks if the role has permission to manage teams.
    /// </summary>
    /// <param name="role">The user role.</param>
    /// <returns>True if the role can manage teams, false otherwise.</returns>
    public static bool CanManageTeams(this UserRole role) =>
        role is UserRole.Admin or UserRole.ProjectManager;

    /// <summary>
    /// Checks if the role has permission to manage projects.
    /// </summary>
    /// <param name="role">The user role.</param>
    /// <returns>True if the role can manage projects, false otherwise.</returns>
    public static bool CanManageProjects(this UserRole role) =>
        role is UserRole.Admin or UserRole.ProjectManager;

    /// <summary>
    /// Checks if the role can be assigned tasks.
    /// </summary>
    /// <param name="role">The user role.</param>
    /// <returns>True if the role can be assigned tasks, false otherwise.</returns>
    public static bool CanBeAssignedTasks(this UserRole role) =>
        role is UserRole.Developer or UserRole.ProjectManager or UserRole.Admin;

    /// <summary>
    /// Checks if the role has administrative privileges.
    /// </summary>
    /// <param name="role">The user role.</param>
    /// <returns>True if the role has admin privileges, false otherwise.</returns>
    public static bool IsAdmin(this UserRole role) =>
        role == UserRole.Admin;

    /// <summary>
    /// Checks if the role has management privileges.
    /// </summary>
    /// <param name="role">The user role.</param>
    /// <returns>True if the role has management privileges, false otherwise.</returns>
    public static bool HasManagementPrivileges(this UserRole role) =>
        role is UserRole.ProjectManager or UserRole.Admin;

    /// <summary>
    /// Gets the hierarchy level of the role (higher number = more privileges).
    /// </summary>
    /// <param name="role">The user role.</param>
    /// <returns>A numeric value representing the hierarchy level.</returns>
    public static int GetHierarchyLevel(this UserRole role) => role switch
    {
        UserRole.Viewer => 1,
        UserRole.Developer => 2,
        UserRole.ProjectManager => 3,
        UserRole.Admin => 4,
        _ => 0
    };

    /// <summary>
    /// Checks if the current role has higher or equal privileges than the target role.
    /// </summary>
    /// <param name="currentRole">The current user role.</param>
    /// <param name="targetRole">The target role to compare against.</param>
    /// <returns>True if current role has higher or equal privileges, false otherwise.</returns>
    public static bool HasHigherOrEqualPrivileges(this UserRole currentRole, UserRole targetRole) =>
        currentRole.GetHierarchyLevel() >= targetRole.GetHierarchyLevel();

    /// <summary>
    /// Gets the permissions associated with the role.
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
        _ => Array.Empty<string>()
    };
}
