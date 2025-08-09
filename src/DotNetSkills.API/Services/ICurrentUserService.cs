namespace DotNetSkills.API.Services;

/// <summary>
/// Service for extracting current user context from JWT claims.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user ID from JWT claims.
    /// </summary>
    /// <returns>The current user ID, or null if not authenticated.</returns>
    UserId? GetCurrentUserId();

    /// <summary>
    /// Gets the current user's role from JWT claims.
    /// </summary>
    /// <returns>The current user's role, or null if not authenticated.</returns>
    UserRole? GetCurrentUserRole();

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    /// <returns>True if authenticated, false otherwise.</returns>
    bool IsAuthenticated();
}