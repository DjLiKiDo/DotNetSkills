namespace DotNetSkills.Application.ProjectManagement.DTOs;

/// <summary>
/// Response DTO for paginated project task results.
/// Contains the list of project tasks with pagination metadata and project context.
/// </summary>
public record PagedProjectTaskResponse(
    IReadOnlyList<ProjectTaskResponse> Tasks,
    int TotalCount,
    int Page,
    int PageSize,
    Guid ProjectId,
    string ProjectName,
    int ActiveTaskCount,
    int CompletedTaskCount,
    int OverdueTaskCount
)
{
    /// <summary>
    /// Calculates the total number of pages based on page size.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Indicates whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Indicates whether there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Gets the starting item number for the current page.
    /// </summary>
    public int StartItem => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;

    /// <summary>
    /// Gets the ending item number for the current page.
    /// </summary>
    public int EndItem => Math.Min(Page * PageSize, TotalCount);

    /// <summary>
    /// Gets the project completion percentage based on task completion.
    /// </summary>
    public decimal ProjectCompletionPercentage => TotalCount == 0 ? 0m : (decimal)CompletedTaskCount / TotalCount * 100m;
}