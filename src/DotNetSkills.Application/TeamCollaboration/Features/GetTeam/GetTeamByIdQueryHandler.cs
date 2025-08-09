namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeam;

/// <summary>
/// Handler for GetTeamByIdQuery that retrieves team details using repository pattern.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class GetTeamByIdQueryHandler : IRequestHandler<GetTeamByIdQuery, TeamResponse>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMapper _mapper;

    public GetTeamByIdQueryHandler(
        ITeamRepository teamRepository,
        IMapper mapper)
    {
        _teamRepository = teamRepository;
        _mapper = mapper;
    }

    public async Task<TeamResponse> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetWithMembersAsync(request.TeamId, cancellationToken).ConfigureAwait(false);
        if (team == null)
            throw new InvalidOperationException($"Team with ID '{request.TeamId}' not found");

        return _mapper.Map<TeamResponse>(team);
    }
}
