namespace DotNetSkills.Application.ProjectManagement.Queries;

/// <summary>
/// Query for retrieving projects with pagination and filtering.
/// Supports filtering by team, status, date ranges, and search.
/// </summary>
public record GetProjectsQuery(
    int Page, 
    int PageSize, 
    TeamId? TeamId,
    ProjectStatus? Status,
    DateTime? StartDateFrom,
    DateTime? StartDateTo,
    DateTime? EndDateFrom,
    DateTime? EndDateTo,
    string? Search) : IRequest<PagedProjectResponse>
{
    /// <summary>
    /// Validates the query parameters.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (Page < 1)
            throw new ArgumentException("Page must be greater than 0.", nameof(Page));

        if (PageSize < 1 || PageSize > 100)
            throw new ArgumentException("PageSize must be between 1 and 100.", nameof(PageSize));

        if (StartDateFrom.HasValue && StartDateTo.HasValue && StartDateFrom > StartDateTo)
            throw new ArgumentException("Start date from cannot be greater than start date to.", nameof(StartDateFrom));

        if (EndDateFrom.HasValue && EndDateTo.HasValue && EndDateFrom > EndDateTo)
            throw new ArgumentException("End date from cannot be greater than end date to.", nameof(EndDateFrom));

        if (Status.HasValue && !Enum.IsDefined(typeof(ProjectStatus), Status.Value))
            throw new ArgumentException("Invalid project status.", nameof(Status));
    }
}

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, PagedProjectResponse>
{
    public async Task<PagedProjectResponse> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement project retrieval logic with filtering
        // 1. Get projects from repository with pagination and filters
        // 2. Apply team filter if provided
        // 3. Apply status filter if provided
        // 4. Apply date range filters if provided
        // 5. Apply search filter if provided
        // 6. Map to DTOs and return paginated response
        
        await Task.CompletedTask;
        throw new NotImplementedException("GetProjectsQueryHandler requires Infrastructure layer implementation");
    }
}