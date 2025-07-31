namespace DotNetSkills.Domain.Enums;

public enum UserRole
{
    Viewer = 1,
    Developer = 2,
    ProjectManager = 3,
    Admin = 4
}

public static class UserRoleExtensions
{
    public static bool CanManageUsers(this UserRole role) =>
        role == UserRole.Admin;

    public static bool CanManageTeams(this UserRole role) =>
        role >= UserRole.ProjectManager;

    public static bool CanManageProjects(this UserRole role) =>
        role >= UserRole.ProjectManager;

    public static bool CanManageTasks(this UserRole role) =>
        role >= UserRole.Developer;

    public static bool CanBeAssignedTasks(this UserRole role) =>
        role >= UserRole.Developer;

    public static bool HasHigherOrEqualAuthorityThan(this UserRole role, UserRole otherRole) =>
        role >= otherRole;

    public static string GetDisplayName(this UserRole role) => role switch
    {
        UserRole.Viewer => "Viewer",
        UserRole.Developer => "Developer",
        UserRole.ProjectManager => "Project Manager",
        UserRole.Admin => "Administrator",
        _ => role.ToString()
    };
}