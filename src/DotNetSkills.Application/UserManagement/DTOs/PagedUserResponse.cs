namespace DotNetSkills.Application.UserManagement.DTOs;

/// <summary>
/// Response DTO for paginated user lists.
/// This DTO provides pagination metadata along with user data.
/// </summary>
public record PagedUserResponse(
    IReadOnlyList<UserResponse> Users,
    int TotalCount,
    int Page,
    int PageSize)
{
    /// <summary>
    /// Total number of pages based on page size.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    
    /// <summary>
    /// Indicates whether there is a next page available.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
    
    /// <summary>
    /// Indicates whether there is a previous page available.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
