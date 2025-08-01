namespace DotNetSkills.Domain.UserManagement.Enums;

/// <summary>
/// Represents the role of a user in the system for authorization purposes.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// A viewer who can only view information without modification rights.
    /// </summary>
    Viewer = 1,

    /// <summary>
    /// A developer who can be assigned tasks and work on projects.
    /// </summary>
    Developer = 2,

    /// <summary>
    /// A project manager who can manage projects and assign tasks.
    /// </summary>
    ProjectManager = 3,

    /// <summary>
    /// An administrator with full system access and user management capabilities.
    /// </summary>
    Admin = 4
}
