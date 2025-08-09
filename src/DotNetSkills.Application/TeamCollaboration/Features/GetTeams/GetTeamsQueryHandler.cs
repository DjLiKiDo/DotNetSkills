namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeams;

/// <summary>
/// Handler for GetTeamsQuery that retrieves teams with pagination and search using repository pattern.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, PagedTeamResponse>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMapper _mapper;

    public GetTeamsQueryHandler(ITeamRepository teamRepository, IMapper mapper)
    {
        _teamRepository = teamRepository;
        _mapper = mapper;
    }

    public async Task<PagedTeamResponse> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
    {
        var (teams, totalCount) = await _teamRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            status: null,
            cancellationToken).ConfigureAwait(false);

        var teamResponses = teams.Select(team => _mapper.Map<TeamResponse>(team)).ToList().AsReadOnly();

        return new PagedTeamResponse(
            teamResponses,
            totalCount,
            request.Page,
            request.PageSize);
    }
}
