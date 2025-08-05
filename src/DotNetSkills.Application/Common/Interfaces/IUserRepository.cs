namespace DotNetSkills.Application.Common.Interfaces;

/// <summary>
/// Repository interface specific to User entities.
/// Extends the generic repository with User-specific query methods.
/// </summary>
public interface IUserRepository : IRepository<User, UserId>
{
    /// <summary>
    /// Gets a user by their email address asynchronously.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the specified email address exists.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a user with the email exists, otherwise false.</returns>
    Task<bool> ExistsByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by their role asynchronously.
    /// </summary>
    /// <param name="role">The user role to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of users with the specified role.</returns>
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by their status asynchronously.
    /// </summary>
    /// <param name="status">The user status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of users with the specified status.</returns>
    Task<IEnumerable<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);

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
    Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        UserRole? role = null,
        UserStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users who are members of a specific team asynchronously.
    /// </summary>
    /// <param name="teamId">The team ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of users who are members of the specified team.</returns>
    Task<IEnumerable<User>> GetByTeamMembershipAsync(TeamId teamId, CancellationToken cancellationToken = default);
}