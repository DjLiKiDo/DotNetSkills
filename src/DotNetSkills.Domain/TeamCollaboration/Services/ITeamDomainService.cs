namespace DotNetSkills.Domain.TeamCollaboration.Services;

/// <summary>
/// Domain service for complex team management operations that require external dependencies
/// or cross-aggregate coordination. This service handles team-related business logic that
/// spans multiple aggregates or requires repository access.
/// </summary>
/// <remarks>
/// This interface defines contracts for operations that:
/// - Require database access to check team dependencies
/// - Involve cross-aggregate business logic (teams, projects, users)
/// - Orchestrate complex team management operations
/// 
/// Simple team business rules remain in BusinessRules.TeamManagement for performance.
/// </remarks>
public interface ITeamDomainService
{
    /// <summary>
    /// Validates if a team can be safely deleted from the system.
    /// This involves checking for active projects, team members, and other dependencies.
    /// </summary>
    /// <param name="teamId">The ID of the team to validate for deletion.</param>
    /// <param name="requestingUser">The user requesting the deletion (for authorization).</param>
    /// <returns>True if the team can be deleted, false otherwise.</returns>
    Task<bool> CanDeleteTeamAsync(TeamId teamId, User requestingUser);

    /// <summary>
    /// Checks if a team has any active projects that would prevent certain operations.
    /// </summary>
    /// <param name="teamId">The ID of the team to check.</param>
    /// <returns>True if the team has active projects, false otherwise.</returns>
    Task<bool> HasActiveProjectsAsync(TeamId teamId);

    /// <summary>
    /// Validates if a user can be added to a team considering current capacity,
    /// user availability, and business rules.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="userId">The ID of the user to add.</param>
    /// <param name="role">The role the user would have in the team.</param>
    /// <param name="requestingUser">The user making the request.</param>
    /// <returns>True if the user can be added to the team, false otherwise.</returns>
    Task<bool> CanAddMemberAsync(TeamId teamId, UserId userId, TeamRole role, User requestingUser);

    /// <summary>
    /// Validates if a user can be removed from a team considering active assignments
    /// and project dependencies.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <param name="requestingUser">The user making the request.</param>
    /// <returns>True if the user can be removed from the team, false otherwise.</returns>
    Task<bool> CanRemoveMemberAsync(TeamId teamId, UserId userId, User requestingUser);

    /// <summary>
    /// Calculates the current workload and capacity metrics for a team.
    /// This includes active projects, task assignments, and member availability.
    /// </summary>
    /// <param name="teamId">The ID of the team to analyze.</param>
    /// <returns>The team capacity analysis result.</returns>
    Task<TeamCapacityResult> CalculateTeamCapacityAsync(TeamId teamId);

    /// <summary>
    /// Validates team name uniqueness across the system.
    /// </summary>
    /// <param name="name">The team name to validate.</param>
    /// <param name="excludeTeamId">Optional team ID to exclude from the check (for updates).</param>
    /// <returns>True if the team name is unique, false if it already exists.</returns>
    Task<bool> IsTeamNameUniqueAsync(string name, TeamId? excludeTeamId = null);
}

/// <summary>
/// Result object for team capacity analysis.
/// </summary>
public class TeamCapacityResult
{
    /// <summary>
    /// Gets the total number of team members.
    /// </summary>
    public int TotalMembers { get; init; }

    /// <summary>
    /// Gets the number of active team members.
    /// </summary>
    public int ActiveMembers { get; init; }

    /// <summary>
    /// Gets the number of active projects assigned to the team.
    /// </summary>
    public int ActiveProjects { get; init; }

    /// <summary>
    /// Gets the total number of active tasks assigned to team members.
    /// </summary>
    public int ActiveTasks { get; init; }

    /// <summary>
    /// Gets the team's current workload percentage (0-100).
    /// </summary>
    public double WorkloadPercentage { get; init; }

    /// <summary>
    /// Gets whether the team is at capacity for new members.
    /// </summary>
    public bool IsAtCapacity { get; init; }

    /// <summary>
    /// Gets whether the team can accept new projects.
    /// </summary>
    public bool CanAcceptNewProjects { get; init; }

    /// <summary>
    /// Gets additional notes about the team's capacity status.
    /// </summary>
    public string? Notes { get; init; }
}
