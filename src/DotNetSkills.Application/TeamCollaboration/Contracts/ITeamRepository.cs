using DotNetSkills.Application.TeamCollaboration.Projections;

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

    /// <summary>
    /// Gets teams by their status as an async enumerable for streaming large result sets.
    /// Memory-efficient for processing many teams with the specified status.
    /// </summary>
    /// <param name="status">The team status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of teams with the specified status.</returns>
    IAsyncEnumerable<Team> GetByStatusAsyncEnumerable(DotNetSkills.Domain.TeamCollaboration.Enums.TeamStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams that a specific user is a member of as an async enumerable.
    /// Memory-efficient for users who are members of many teams.
    /// </summary>
    /// <param name="userId">The user ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of teams the user is a member of.</returns>
    IAsyncEnumerable<Team> GetByUserMembershipAsyncEnumerable(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active teams as an async enumerable for bulk operations.
    /// Optimized for memory efficiency when processing large numbers of active teams.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of active teams.</returns>
    IAsyncEnumerable<Team> GetActiveTeamsAsyncEnumerable(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets team summaries with optimized projection for read-only scenarios.
    /// Minimizes data transfer by selecting only required fields.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team summary projections.</returns>
    Task<IEnumerable<TeamSummaryProjection>> GetTeamSummariesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets team dashboard information with aggregated data.
    /// Optimized for dashboard scenarios with minimal queries.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team dashboard projections.</returns>
    Task<IEnumerable<TeamDashboardProjection>> GetTeamDashboardDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets team selection data for dropdowns and selection lists.
    /// Minimal projection for UI scenarios.
    /// </summary>
    /// <param name="activeOnly">Whether to return only active teams.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team selection projections.</returns>
    Task<IEnumerable<TeamSelectionProjection>> GetTeamSelectionsAsync(bool activeOnly = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets team membership information for a specific user.
    /// Shows teams the user belongs to with role context.
    /// </summary>
    /// <param name="userId">The user ID to get team memberships for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team membership projections.</returns>
    Task<IEnumerable<TeamMembershipProjection>> GetUserTeamMembershipsAsync(UserId userId, CancellationToken cancellationToken = default);
}
