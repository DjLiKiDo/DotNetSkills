namespace DotNetSkills.Application.TeamCollaboration.Queries;

/// <summary>
/// Query for retrieving teams with pagination and optional search.
/// </summary>
public record GetTeamsQuery(int Page, int PageSize, string? Search) : IRequest<PagedTeamResponse>
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
    }
}

public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, PagedTeamResponse>
{
    public async Task<PagedTeamResponse> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement team retrieval logic with pagination and search
        // 1. Get teams from repository with pagination
        // 2. Apply search filter if provided
        // 3. Map to DTO and return paginated response
        
        await Task.CompletedTask;
        throw new NotImplementedException("GetTeamsQueryHandler requires Infrastructure layer implementation");
    }
}