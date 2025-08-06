namespace DotNetSkills.Application.TeamCollaboration.Contracts.Responses;

/// <summary>
/// Response DTO for paginated team listings.
/// This DTO represents a paginated list of teams returned from API endpoints.
/// </summary>
public record PagedTeamResponse(
    IReadOnlyList<TeamResponse> Teams,
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
}
