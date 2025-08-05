namespace DotNetSkills.Application.UserManagement.Queries;

/// <summary>
/// Query to get users with pagination support.
/// This query returns a paginated list of users with optional filtering.
/// TODO: Implement IRequest&lt;PagedUserResponse&gt; when MediatR is added.
/// </summary>
public record GetUsersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null)
{
    /// <summary>
    /// Validates that pagination parameters are valid.
    /// </summary>
    public void Validate()
    {
        if (Page <= 0)
            throw new ArgumentException("Page must be greater than 0", nameof(Page));

        if (PageSize <= 0 || PageSize > 100)
            throw new ArgumentException("PageSize must be between 1 and 100", nameof(PageSize));
    }

    /// <summary>
    /// Gets the number of items to skip for pagination.
    /// </summary>
    public int Skip => (Page - 1) * PageSize;
}
