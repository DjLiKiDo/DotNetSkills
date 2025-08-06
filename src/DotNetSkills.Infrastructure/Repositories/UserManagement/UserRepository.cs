namespace DotNetSkills.Infrastructure.Repositories.UserManagement;

/// <summary>
/// Entity Framework Core implementation of the IUserRepository interface.
/// Provides data access operations for User entities with optimized queries and caching strategies.
/// </summary>
public class UserRepository : BaseRepository<User, UserId>, IUserRepository
{
    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets a user by their email address asynchronously.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found, otherwise null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when email is null.</exception>
    public async Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));

        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    /// <summary>
    /// Checks if a user with the specified email address exists.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a user with the email exists, otherwise false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when email is null.</exception>
    public async Task<bool> ExistsByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));

        return await DbSet
            .AsNoTracking()
            .AnyAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    /// <summary>
    /// Gets users by their role asynchronously.
    /// </summary>
    /// <param name="role">The user role to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of users with the specified role.</returns>
    /// <exception cref="ArgumentNullException">Thrown when role is null.</exception>
    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(u => u.Role == role)
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets users by their status asynchronously.
    /// </summary>
    /// <param name="status">The user status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of users with the specified status.</returns>
    /// <exception cref="ArgumentNullException">Thrown when status is null.</exception>
    public async Task<IEnumerable<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(u => u.Status == status)
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets users with pagination support asynchronously.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term to filter by name or email.</param>
    /// <param name="role">Optional role filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated collection of users.</returns>
    public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        UserRole? role = null,
        UserStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u => 
                u.Name.Contains(searchTerm) || 
                u.Email.Value.Contains(searchTerm));
        }

        // Apply role filter
        if (role != null)
        {
            query = query.Where(u => u.Role == role);
        }

        // Apply status filter
        if (status != null)
        {
            query = query.Where(u => u.Status == status);
        }

        // Apply ordering
        query = query.OrderBy(u => u.Name);

        // Get paginated results
        var (users, totalCount) = await GetPagedAsync(query, pageNumber, pageSize, cancellationToken);
        
        return (users, totalCount);
    }

    /// <summary>
    /// Gets users who are members of a specific team asynchronously.
    /// </summary>
    /// <param name="teamId">The team ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of users who are members of the specified team.</returns>
    /// <exception cref="ArgumentNullException">Thrown when teamId is null.</exception>
    public async Task<IEnumerable<User>> GetByTeamMembershipAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        if (teamId == null)
            throw new ArgumentNullException(nameof(teamId));

        return await DbSet
            .AsNoTracking()
            .Where(u => u.TeamMemberships.Any(tm => tm.TeamId == teamId))
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a user with their team memberships included asynchronously.
    /// Optimized for scenarios that need user's team information.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user with team memberships if found, otherwise null.</returns>
    public async Task<User?> GetWithTeamMembershipsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        return await DbSet
            .AsNoTracking()
            .Include(u => u.TeamMemberships)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets users with specific roles that are available for team assignment.
    /// Filters out users that might be at capacity or unavailable.
    /// </summary>
    /// <param name="roles">The user roles to filter by.</param>
    /// <param name="excludeTeamId">Optional team ID to exclude users that are already members.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of available users.</returns>
    public async Task<IEnumerable<User>> GetAvailableForTeamAssignmentAsync(
        IEnumerable<UserRole> roles,
        TeamId? excludeTeamId = null,
        CancellationToken cancellationToken = default)
    {
        if (roles == null)
            throw new ArgumentNullException(nameof(roles));

        var rolesList = roles.ToList();
        if (!rolesList.Any())
            return Enumerable.Empty<User>();

        var query = DbSet
            .AsNoTracking()
            .Where(u => u.Status == UserStatus.Active && rolesList.Contains(u.Role));

        // Exclude users already in the specified team
        if (excludeTeamId != null)
        {
            query = query.Where(u => !u.TeamMemberships.Any(tm => tm.TeamId == excludeTeamId));
        }

        return await query
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets user statistics including team memberships and role distribution.
    /// Optimized for dashboard and reporting scenarios.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of user statistics grouped by role and status.</returns>
    public async Task<IEnumerable<(UserRole Role, UserStatus Status, int Count)>> GetUserStatisticsAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .GroupBy(u => new { u.Role, u.Status })
            .Select(g => new ValueTuple<UserRole, UserStatus, int>(g.Key.Role, g.Key.Status, g.Count()))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Override to provide User-specific default ordering.
    /// </summary>
    /// <returns>Expression to order by user name.</returns>
    protected override Expression<Func<User, object>> GetDefaultOrderingExpression()
    {
        return u => u.Name;
    }
}
