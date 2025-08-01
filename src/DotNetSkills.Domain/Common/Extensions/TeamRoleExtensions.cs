namespace DotNetSkills.Domain.Common.Extensions;

/// <summary>
/// Extension methods for TeamRole enum providing business logic and utility methods.
/// </summary>
public static class TeamRoleExtensions
{
    /// <summary>
    /// Gets the display name for the team role.
    /// </summary>
    /// <param name="role">The team role.</param>
    /// <returns>A human-readable display name.</returns>
    public static string GetDisplayName(this TeamRole role) => role switch
    {
        TeamRole.Developer => "Developer",
        TeamRole.ProjectManager => "Project Manager",
        TeamRole.TeamLead => "Team Lead",
        TeamRole.Viewer => "Viewer",
        _ => role.ToString()
    };

    /// <summary>
    /// Checks if the team role has leadership privileges.
    /// </summary>
    /// <param name="role">The team role.</param>
    /// <returns>True if the role has leadership privileges, false otherwise.</returns>
    public static bool HasLeadershipPrivileges(this TeamRole role) =>
        role is TeamRole.TeamLead or TeamRole.ProjectManager;

    /// <summary>
    /// Checks if the team role can assign tasks to other team members.
    /// </summary>
    /// <param name="role">The team role.</param>
    /// <returns>True if the role can assign tasks, false otherwise.</returns>
    public static bool CanAssignTasks(this TeamRole role) =>
        role is TeamRole.ProjectManager or TeamRole.TeamLead;

    /// <summary>
    /// Checks if the team role can be assigned tasks.
    /// </summary>
    /// <param name="role">The team role.</param>
    /// <returns>True if the role can be assigned tasks, false otherwise.</returns>
    public static bool CanBeAssignedTasks(this TeamRole role) =>
        role is TeamRole.Developer or TeamRole.ProjectManager or TeamRole.TeamLead;

    /// <summary>
    /// Checks if the team role can manage team membership.
    /// </summary>
    /// <param name="role">The team role.</param>
    /// <returns>True if the role can manage team membership, false otherwise.</returns>
    public static bool CanManageTeamMembership(this TeamRole role) =>
        role is TeamRole.ProjectManager or TeamRole.TeamLead;

    /// <summary>
    /// Checks if the team role can change roles of other team members.
    /// </summary>
    /// <param name="role">The team role.</param>
    /// <param name="targetRole">The role of the target team member.</param>
    /// <returns>True if the role can change the target role, false otherwise.</returns>
    public static bool CanChangeRole(this TeamRole role, TeamRole targetRole) => role switch
    {
        TeamRole.ProjectManager => true, // Can change any role
        TeamRole.TeamLead => targetRole != TeamRole.ProjectManager, // Cannot change Project Manager roles
        _ => false
    };

    /// <summary>
    /// Gets the hierarchy level of the team role (higher number = more privileges).
    /// </summary>
    /// <param name="role">The team role.</param>
    /// <returns>A numeric value representing the hierarchy level.</returns>
    public static int GetHierarchyLevel(this TeamRole role) => role switch
    {
        TeamRole.Viewer => 1,
        TeamRole.Developer => 2,
        TeamRole.TeamLead => 3,
        TeamRole.ProjectManager => 4,
        _ => 0
    };

    /// <summary>
    /// Checks if the current role has higher or equal privileges than the target role.
    /// </summary>
    /// <param name="currentRole">The current team role.</param>
    /// <param name="targetRole">The target role to compare against.</param>
    /// <returns>True if current role has higher or equal privileges, false otherwise.</returns>
    public static bool HasHigherOrEqualPrivileges(this TeamRole currentRole, TeamRole targetRole) =>
        currentRole.GetHierarchyLevel() >= targetRole.GetHierarchyLevel();

    /// <summary>
    /// Gets the color code associated with the team role for UI purposes.
    /// </summary>
    /// <param name="role">The team role.</param>
    /// <returns>A hexadecimal color code.</returns>
    public static string GetColorCode(this TeamRole role) => role switch
    {
        TeamRole.ProjectManager => "#dc3545", // Red
        TeamRole.TeamLead => "#fd7e14",       // Orange
        TeamRole.Developer => "#007bff",      // Blue
        TeamRole.Viewer => "#6c757d",         // Gray
        _ => "#6c757d"                        // Gray (default)
    };

    /// <summary>
    /// Gets the responsibilities associated with the team role.
    /// </summary>
    /// <param name="role">The team role.</param>
    /// <returns>A collection of responsibility descriptions.</returns>
    public static IReadOnlyList<string> GetResponsibilities(this TeamRole role) => role switch
    {
        TeamRole.ProjectManager => new[]
        {
            "Manage project scope and timeline",
            "Assign and reassign tasks",
            "Manage team membership and roles",
            "Report to stakeholders"
        },
        TeamRole.TeamLead => new[]
        {
            "Lead technical decisions",
            "Mentor team members",
            "Assign tasks to developers",
            "Review code and technical work"
        },
        TeamRole.Developer => new[]
        {
            "Complete assigned tasks",
            "Participate in code reviews",
            "Collaborate with team members",
            "Contribute to technical discussions"
        },
        TeamRole.Viewer => new[]
        {
            "View project progress",
            "Access reports and metrics",
            "Observe team activities"
        },
        _ => Array.Empty<string>()
    };

    /// <summary>
    /// Gets an icon name associated with the team role for UI purposes.
    /// </summary>
    /// <param name="role">The team role.</param>
    /// <returns>An icon name string (using common icon library conventions).</returns>
    public static string GetIconName(this TeamRole role) => role switch
    {
        TeamRole.ProjectManager => "briefcase",
        TeamRole.TeamLead => "users",
        TeamRole.Developer => "code",
        TeamRole.Viewer => "eye",
        _ => "user"
    };
}
