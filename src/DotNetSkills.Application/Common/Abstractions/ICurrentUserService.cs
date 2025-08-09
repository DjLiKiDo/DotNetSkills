namespace DotNetSkills.Application.Common.Abstractions;

/// <summary>
/// Cross-cutting abstraction for accessing the currently authenticated user context.
/// Located under Common.Abstractions because:
/// 1. It's a pure contract (no logic) consumed by multiple features.
/// 2. Implementations depend on outer concerns (HTTP/JWT) which live outside Application.
/// 3. Co-location with other infrastructural contracts (IUnitOfWork, IDomainEventDispatcher) aids discoverability.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user ID.
    /// </summary>
    /// <returns>UserId or null if unauthenticated.</returns>
    UserId? GetCurrentUserId();

    /// <summary>
    /// Gets the current user's primary role.
    /// </summary>
    /// <returns>UserRole or null if unauthenticated.</returns>
    UserRole? GetCurrentUserRole();

    /// <summary>
    /// True if a user identity is authenticated.
    /// </summary>
    bool IsAuthenticated();
}
