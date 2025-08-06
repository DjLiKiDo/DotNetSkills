namespace DotNetSkills.Infrastructure.Repositories.TeamCollaboration;

/// <summary>
/// Entity Framework Core implementation of the ITeamRepository interface.
/// Provides data access operations for Team entities with member management and optimization strategies.
/// </summary>
public class TeamRepository : BaseRepository<Team, TeamId>, ITeamRepository
{
    /// <summary>
    /// Initializes a new instance of the TeamRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TeamRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets a team by its name asynchronously.
    /// </summary>
    /// <param name="name">The team name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The team if found, otherwise null.</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
    public async Task<Team?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name cannot be null or empty.", nameof(name));

        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    /// <summary>
    /// Checks if a team with the specified name exists.
    /// </summary>
    /// <param name="name">The team name to check.</param>
    /// <param name="excludeTeamId">Optional team ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a team with the name exists, otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
    public async Task<bool> ExistsByNameAsync(string name, TeamId? excludeTeamId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name cannot be null or empty.", nameof(name));

        var query = DbSet.AsNoTracking().Where(t => t.Name == name);

        if (excludeTeamId != null)
        {
            query = query.Where(t => t.Id != excludeTeamId);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a team with its members included asynchronously.
    /// </summary>
    /// <param name="id">The team ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The team with members if found, otherwise null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when id is null.</exception>
    public async Task<Team?> GetWithMembersAsync(TeamId id, CancellationToken cancellationToken = default)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        return await DbSet
            .AsNoTracking()
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets teams by their status asynchronously.
    /// </summary>
    /// <param name="status">The team status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams with the specified status.</returns>
    /// <exception cref="ArgumentNullException">Thrown when status is null.</exception>
    public async Task<IEnumerable<Team>> GetByStatusAsync(TeamStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(t => t.Status == status)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets teams that a specific user is a member of asynchronously.
    /// </summary>
    /// <param name="userId">The user ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams the user is a member of.</returns>
    /// <exception cref="ArgumentNullException">Thrown when userId is null.</exception>
    public async Task<IEnumerable<Team>> GetByUserMembershipAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        if (userId == null)
            throw new ArgumentNullException(nameof(userId));

        return await DbSet
            .AsNoTracking()
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets teams with pagination support asynchronously.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term to filter by name or description.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated collection of teams with member counts.</returns>
    public async Task<(IEnumerable<Team> Teams, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        TeamStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => 
                t.Name.Contains(searchTerm) || 
                (t.Description != null && t.Description.Contains(searchTerm)));
        }

        // Apply status filter
        if (status != null)
        {
            query = query.Where(t => t.Status == status);
        }

        // Apply ordering
        query = query.OrderBy(t => t.Name);

        // Get paginated results
        var (teams, totalCount) = await GetPagedAsync(query, pageNumber, pageSize, cancellationToken);
        
        return (teams, totalCount);
    }

    /// <summary>
    /// Gets teams with their member counts asynchronously.
    /// Optimized for dashboard and overview scenarios.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams with member count information.</returns>
    public async Task<IEnumerable<(Team Team, int MemberCount)>> GetWithMemberCountsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Select(t => new 
            {
                Team = t,
                MemberCount = t.Members.Count()
            })
            .OrderBy(x => x.Team.Name)
            .ToListAsync(cancellationToken)
            .ContinueWith(task => task.Result.Select(x => (x.Team, x.MemberCount)), cancellationToken);
    }

    /// <summary>
    /// Gets teams that have capacity to accept new members.
    /// Filters teams that haven't reached their maximum member limit.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams that can accept new members.</returns>
    public async Task<IEnumerable<Team>> GetTeamsWithCapacityAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(t => t.Status == TeamStatus.Active && t.Members.Count() < ValidationConstants.Numeric.TeamMaxMembers)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets team members for a specific team with their roles.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="role">Optional role filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team members with user information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when teamId is null.</exception>
    public async Task<IEnumerable<TeamMember>> GetTeamMembersAsync(
        TeamId teamId, 
        TeamRole? role = null, 
        CancellationToken cancellationToken = default)
    {
        if (teamId == null)
            throw new ArgumentNullException(nameof(teamId));

        var query = Context.Set<TeamMember>()
            .AsNoTracking()
            .Where(tm => tm.TeamId == teamId);

        if (role != null)
        {
            query = query.Where(tm => tm.Role == role);
        }

        return await query
            .OrderBy(tm => tm.UserId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets teams with their project counts for dashboard scenarios.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams with project count information.</returns>
    public async Task<IEnumerable<(Team Team, int ProjectCount, int ActiveProjectCount)>> GetWithProjectCountsAsync(
        TeamStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (status != null)
        {
            query = query.Where(t => t.Status == status);
        }

        return (await query
            .Select(t => new 
            {
                Team = t,
                ProjectCount = Context.Set<Project>().Count(p => p.TeamId == t.Id),
                ActiveProjectCount = Context.Set<Project>().Count(p => p.TeamId == t.Id && 
                    p.Status == ProjectStatus.Active)
            })
            .OrderBy(x => x.Team.Name)
            .ToListAsync(cancellationToken))
            .Select(x => (x.Team, x.ProjectCount, x.ActiveProjectCount));
    }

    /// <summary>
    /// Gets teams ordered by their activity level (project count and member count).
    /// Useful for team performance analytics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams ordered by activity level.</returns>
    public async Task<IEnumerable<Team>> GetOrderedByActivityAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(t => t.Status == TeamStatus.Active)
            .OrderByDescending(t => Context.Set<Project>().Count(p => p.TeamId == t.Id))
            .ThenByDescending(t => t.Members.Count())
            .ThenBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Override to provide Team-specific default ordering.
    /// </summary>
    /// <returns>Expression to order by team name.</returns>
    protected override Expression<Func<Team, object>> GetDefaultOrderingExpression()
    {
        return t => t.Name;
    }
}
