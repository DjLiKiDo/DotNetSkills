namespace DotNetSkills.Application.TeamCollaboration.Contracts;

/// <summary>
/// Repository interface specific to Team entities.
/// Extends the generic repository with Team-specific query methods for team management operations.
/// </summary>
public interface ITeamRepository : IRepository<Team, TeamId>
{
    /// <summary>
    /// Gets a team by its name asynchronously.
    /// </summary>
    /// <param name="name">The team name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The team if found, otherwise null.</returns>
    Task<Team?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a team with the specified name exists.
    /// </summary>
    /// <param name="name">The team name to check.</param>
    /// <param name="excludeTeamId">Optional team ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a team with the name exists, otherwise false.</returns>
    Task<bool> ExistsByNameAsync(string name, TeamId? excludeTeamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a team with its members included asynchronously.
    /// </summary>
    /// <param name="id">The team ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The team with members if found, otherwise null.</returns>
    Task<Team?> GetWithMembersAsync(TeamId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams by their status asynchronously.
    /// </summary>
    /// <param name="status">The team status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams with the specified status.</returns>
    Task<IEnumerable<Team>> GetByStatusAsync(DotNetSkills.Domain.TeamCollaboration.Enums.TeamStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams that a specific user is a member of asynchronously.
    /// </summary>
    /// <param name="userId">The user ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams the user is a member of.</returns>
    Task<IEnumerable<Team>> GetByUserMembershipAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams with pagination support asynchronously.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term to filter by name or description.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated collection of teams with member counts.</returns>
    Task<(IEnumerable<Team> Teams, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        DotNetSkills.Domain.TeamCollaboration.Enums.TeamStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams with their member counts asynchronously.
    /// Optimized for dashboard and overview scenarios.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams with member count information.</returns>
    Task<IEnumerable<(Team Team, int MemberCount)>> GetWithMemberCountsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams that have capacity to accept new members.
    /// Filters teams that haven't reached their maximum member limit.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams that can accept new members.</returns>
    Task<IEnumerable<Team>> GetTeamsWithCapacityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets team members for a specific team with their roles.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="role">Optional role filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team members with user information.</returns>
    Task<IEnumerable<TeamMember>> GetTeamMembersAsync(
        TeamId teamId, 
        TeamRole? role = null, 
        CancellationToken cancellationToken = default);
}
