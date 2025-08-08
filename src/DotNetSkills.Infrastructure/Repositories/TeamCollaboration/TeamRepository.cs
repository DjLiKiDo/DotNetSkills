using DotNetSkills.Application.TeamCollaboration.Contracts;
using DotNetSkills.Application.TeamCollaboration.Projections;
using DotNetSkills.Domain.ProjectManagement.Entities;
using DotNetSkills.Domain.ProjectManagement.Enums;
using DotNetSkills.Domain.TeamCollaboration.Entities;
using DotNetSkills.Domain.TeamCollaboration.Enums;
using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
using DotNetSkills.Domain.UserManagement.ValueObjects;
using DotNetSkills.Infrastructure.Persistence;
using DotNetSkills.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;

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
        var teamData = await DbSet
            .AsNoTracking()
            .Select(t => new 
            {
                Team = t,
                MemberCount = t.Members.Count()
            })
            .OrderBy(x => x.Team.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return teamData.Select(x => (x.Team, x.MemberCount));
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
    /// Gets teams by their status as an async enumerable for streaming large result sets.
    /// Memory-efficient for processing many teams with the specified status.
    /// </summary>
    /// <param name="status">The team status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of teams with the specified status.</returns>
    public IAsyncEnumerable<Team> GetByStatusAsyncEnumerable(TeamStatus status, CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Where(t => t.Status == status)
            .OrderBy(t => t.Name)
            .AsAsyncEnumerable();
    }

    /// <summary>
    /// Gets teams that a specific user is a member of as an async enumerable.
    /// Memory-efficient for users who are members of many teams.
    /// </summary>
    /// <param name="userId">The user ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of teams the user is a member of.</returns>
    public IAsyncEnumerable<Team> GetByUserMembershipAsyncEnumerable(UserId userId, CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .OrderBy(t => t.Name)
            .AsAsyncEnumerable();
    }

    /// <summary>
    /// Gets all active teams as an async enumerable for bulk operations.
    /// Optimized for memory efficiency when processing large numbers of active teams.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of active teams.</returns>
    public IAsyncEnumerable<Team> GetActiveTeamsAsyncEnumerable(CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Where(t => t.Status == TeamStatus.Active)
            .OrderBy(t => t.Name)
            .AsAsyncEnumerable();
    }

    /// <summary>
    /// Gets team summaries with optimized projection for read-only scenarios.
    /// Minimizes data transfer by selecting only required fields.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team summary projections.</returns>
    public async Task<IEnumerable<TeamSummaryProjection>> GetTeamSummariesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Select(t => new TeamSummaryProjection
            {
                Id = t.Id.Value,
                Name = t.Name,
                Description = t.Description ?? string.Empty,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                MemberCount = t.Members.Count,
                ProjectCount = Context.Set<Project>().Count(p => p.TeamId == t.Id)
            })
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets team dashboard information with aggregated data.
    /// Optimized for dashboard scenarios with minimal queries.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team dashboard projections.</returns>
    public async Task<IEnumerable<TeamDashboardProjection>> GetTeamDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(t => t.Status == TeamStatus.Active)
            .Select(t => new TeamDashboardProjection
            {
                Id = t.Id.Value,
                Name = t.Name,
                Description = t.Description ?? string.Empty,
                Status = t.Status,
                MemberCount = t.Members.Count,
                ActiveProjectCount = Context.Set<Project>().Count(p => p.TeamId == t.Id && p.Status == ProjectStatus.Active),
                CompletedProjectCount = Context.Set<Project>().Count(p => p.TeamId == t.Id && p.Status == ProjectStatus.Completed),
                ActiveTaskCount = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(task => Context.Set<Project>().Any(p => p.TeamId == t.Id && p.Id == task.ProjectId) 
                                   && task.Status != DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done),
                CompletedTaskCount = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(task => Context.Set<Project>().Any(p => p.TeamId == t.Id && p.Id == task.ProjectId) 
                                   && task.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets team selection data for dropdowns and selection lists.
    /// Minimal projection for UI scenarios.
    /// </summary>
    /// <param name="activeOnly">Whether to return only active teams.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team selection projections.</returns>
    public async Task<IEnumerable<TeamSelectionProjection>> GetTeamSelectionsAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (activeOnly)
            query = query.Where(t => t.Status == TeamStatus.Active);

        return await query
            .Select(t => new TeamSelectionProjection
            {
                Id = t.Id.Value,
                Name = t.Name,
                Status = t.Status,
                IsActive = t.Status == TeamStatus.Active,
                MemberCount = t.Members.Count
            })
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets team membership information for a specific user.
    /// Shows teams the user belongs to with role context.
    /// </summary>
    /// <param name="userId">The user ID to get team memberships for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team membership projections.</returns>
    public async Task<IEnumerable<TeamMembershipProjection>> GetUserTeamMembershipsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .Select(t => new TeamMembershipProjection
            {
                TeamId = t.Id.Value,
                TeamName = t.Name,
                TeamStatus = t.Status,
                MemberRole = t.Members.First(m => m.UserId == userId).Role,
                JoinedAt = t.Members.First(m => m.UserId == userId).CreatedAt,
                TotalMembers = t.Members.Count
            })
            .OrderBy(t => t.TeamName)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
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
