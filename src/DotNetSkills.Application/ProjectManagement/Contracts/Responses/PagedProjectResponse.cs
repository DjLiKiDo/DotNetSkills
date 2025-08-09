namespace DotNetSkills.Application.ProjectManagement.Contracts.Responses;

/// <summary>
/// Response DTO for paginated project listings.
/// This DTO represents a paginated list of projects returned from API endpoints.
/// </summary>
public record PagedProjectResponse(
    IReadOnlyList<ProjectResponse> Projects,
    int TotalCount,
    int Page,
    int PageSize)
{
    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets whether there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Gets whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Gets the count of active projects in the current page.
    /// </summary>
    public int ActiveProjectsCount => Projects.Count(p => p.IsActive);

    /// <summary>
    /// Gets the count of overdue projects in the current page.
    /// </summary>
    public int OverdueProjectsCount => Projects.Count(p => p.IsOverdue);
}
