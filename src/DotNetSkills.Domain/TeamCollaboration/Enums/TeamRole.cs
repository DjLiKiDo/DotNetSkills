namespace DotNetSkills.Domain.TeamCollaboration.Enums;

/// <summary>
/// Represents the role of a team member within a team.
/// </summary>
public enum TeamRole
{
    /// <summary>
    /// A developer who can be assigned tasks and work on projects.
    /// </summary>
    Developer = 1,

    /// <summary>
    /// A project manager who can manage projects, assign tasks, and oversee team activities.
    /// </summary>
    ProjectManager = 2,

    /// <summary>
    /// A team lead who can manage team members and has elevated permissions within the team.
    /// </summary>
    TeamLead = 3,

    /// <summary>
    /// A viewer who can only view project and task information without modification rights.
    /// </summary>
    Viewer = 4
}
