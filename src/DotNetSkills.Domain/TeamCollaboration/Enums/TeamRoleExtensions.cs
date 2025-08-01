namespace DotNetSkills.Domain.TeamCollaboration.Enums;

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
