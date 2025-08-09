namespace DotNetSkills.API.Authorization;

/// <summary>
/// Central repository for authorization policy names used throughout the API.
/// </summary>
public static class Policies
{
    /// <summary>
    /// Policy that requires the user to have the Admin role.
    /// </summary>
    public const string AdminOnly = nameof(AdminOnly);

    /// <summary>
    /// Policy that requires the user to have Admin or TeamManager role.
    /// </summary>
    public const string TeamManager = nameof(TeamManager);

    /// <summary>
    /// Policy that requires the user to have Admin or ProjectManager role.
    /// </summary>
    public const string ProjectManagerOrAdmin = nameof(ProjectManagerOrAdmin);

    /// <summary>
    /// Policy that requires the user to have Admin, ProjectManager, or Developer role,
    /// or explicit project membership.
    /// </summary>
    public const string ProjectMemberOrAdmin = nameof(ProjectMemberOrAdmin);
}