namespace DotNetSkills.Application.UserManagement.Features.GetUsers;

/// <summary>
/// Query to retrieve users with pagination, filtering, and search capabilities.
/// Supports role filtering, status filtering, and case-insensitive search by name and email.
/// </summary>
/// <param name="Page">The page number (1-based indexing).</param>
/// <param name="PageSize">The number of items per page (1-100).</param>
/// <param name="SearchTerm">Optional search term to filter by name or email (case-insensitive).</param>
/// <param name="Role">Optional role filter to include only users with the specified role.</param>
/// <param name="Status">Optional status filter to include only users with the specified status.</param>
public record GetUsersQuery(
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    UserRole? Role = null,
    UserStatus? Status = null) : IRequest<PagedUserResponse>
{
    /// <summary>
    /// Gets the number of items to skip for pagination.
    /// </summary>
    public int Skip => (Page - 1) * PageSize;
}
