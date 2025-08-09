using System.Security.Claims;

namespace DotNetSkills.API.Services;

/// <summary>
/// Implementation of ICurrentUserService that extracts user context from JWT claims.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current user ID from JWT claims using the "sub" claim or custom "userId" claim.
    /// </summary>
    public UserId? GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        // Try standard "sub" claim first, then custom "userId" claim
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? 
                         user.FindFirst("sub") ?? 
                         user.FindFirst("userId");

        if (userIdClaim?.Value != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return new UserId(userId);
        }

        return null;
    }

    /// <summary>
    /// Gets the current user's role from JWT claims using the "role" claim.
    /// </summary>
    public UserRole? GetCurrentUserRole()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var roleClaim = user.FindFirst(ClaimTypes.Role) ?? user.FindFirst("role");
        
        if (roleClaim?.Value != null && Enum.TryParse<UserRole>(roleClaim.Value, true, out var role))
        {
            return role;
        }

        return null;
    }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }
}