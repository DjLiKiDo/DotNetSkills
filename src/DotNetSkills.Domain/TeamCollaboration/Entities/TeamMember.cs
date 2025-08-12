namespace DotNetSkills.Domain.TeamCollaboration.Entities;

/// <summary>
/// Represents a user's membership in a team with a specific role.
/// This is a join entity that is part of the Team aggregate.
/// </summary>
public class TeamMember : BaseEntity<TeamMemberId>
{
    /// <summary>
    /// Gets the ID of the user who is a member of the team.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the ID of the team the user is a member of.
    /// </summary>
    public TeamId TeamId { get; private set; }

    /// <summary>
    /// Gets the role of the user within the team.
    /// </summary>
    public TeamRole Role { get; private set; }

    /// <summary>
    /// Gets the date when the user joined the team.
    /// </summary>
    public DateTime JoinedAt { get; private set; }

    // Dependent navigations for EF Core relationship consolidation
    // These are internal to the aggregate and not intended for application orchestration
    public User User { get; private set; } = null!;
    public Team Team { get; private set; } = null!;

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private TeamMember() : base(TeamMemberId.New())
    {
        UserId = null!;
        TeamId = null!;
    }

    /// <summary>
    /// Initializes a new instance of the TeamMember class.
    /// This should only be called from within the Team aggregate.
    /// </summary>
    /// <param name="userId">The ID of the user joining the team.</param>
    /// <param name="teamId">The ID of the team being joined.</param>
    /// <param name="role">The role assigned to the user in the team.</param>
    /// <exception cref="ArgumentNullException">Thrown when userId or teamId is null.</exception>
    internal TeamMember(UserId userId, TeamId teamId, TeamRole role) : base(TeamMemberId.New())
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        TeamId = teamId ?? throw new ArgumentNullException(nameof(teamId));
        Role = role;
        JoinedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Changes the role of the team member.
    /// </summary>
    /// <param name="newRole">The new role to assign to the team member.</param>
    /// <param name="changedBy">The user making the role change (must have appropriate permissions).</param>
    /// <exception cref="DomainException">Thrown when the changer doesn't have permission to change roles.</exception>
    internal void ChangeRole(TeamRole newRole, User changedBy)
    {
        if (!CanChangeRole(changedBy))
            throw new DomainException("User does not have permission to change team member roles");

        Role = newRole;
    }

    /// <summary>
    /// Checks if the specified user can change the role of this team member.
    /// </summary>
    /// <param name="user">The user attempting to change the role.</param>
    /// <returns>True if the user can change the role, false otherwise.</returns>
    private bool CanChangeRole(User user)
    {
        // Admin can always change roles
        if (user.Role == UserRole.Admin)
            return true;

        // System-level Project managers can change roles
        if (user.Role == UserRole.ProjectManager)
            return true;

        // Team-level project managers can change roles within their teams
        if (user.GetRoleInTeam(TeamId) == TeamRole.ProjectManager)
            return true;

        // Team leads can change roles within their teams (except other team leads and project managers)
        if (user.GetRoleInTeam(TeamId) == TeamRole.TeamLead &&
            Role != TeamRole.TeamLead &&
            Role != TeamRole.ProjectManager)
            return true;

        return false;
    }

    /// <summary>
    /// Checks if this team member has leadership privileges (Team Lead or Project Manager).
    /// </summary>
    /// <returns>True if the member has leadership privileges, false otherwise.</returns>
    public bool HasLeadershipPrivileges()
    {
        return Role == TeamRole.TeamLead || Role == TeamRole.ProjectManager;
    }

    /// <summary>
    /// Checks if this team member can be assigned tasks.
    /// </summary>
    /// <returns>True if the member can be assigned tasks, false otherwise.</returns>
    public bool CanBeAssignedTasks()
    {
        return Role == TeamRole.Developer ||
               Role == TeamRole.ProjectManager ||
               Role == TeamRole.TeamLead;
    }

    /// <summary>
    /// Checks if this team member can assign tasks to other members.
    /// </summary>
    /// <returns>True if the member can assign tasks, false otherwise.</returns>
    public bool CanAssignTasks()
    {
        return Role == TeamRole.ProjectManager || Role == TeamRole.TeamLead;
    }
}
