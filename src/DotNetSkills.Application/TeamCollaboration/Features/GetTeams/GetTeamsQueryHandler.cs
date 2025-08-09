namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeams;

/// <summary>
/// Handler for GetTeamsQuery that retrieves teams with pagination and search using repository pattern.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, PagedTeamResponse>
{
    private readonly ITeamRepository _teamRepository;

    public GetTeamsQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<PagedTeamResponse> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
    {
        var (teams, totalCount) = await _teamRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            status: null,
            cancellationToken).ConfigureAwait(false);

        var teamResponses = teams.Select(TeamResponse.FromDomain).ToList().AsReadOnly();

        return new PagedTeamResponse(
            teamResponses,
            totalCount,
            request.Page,
            request.PageSize);
    }
}
