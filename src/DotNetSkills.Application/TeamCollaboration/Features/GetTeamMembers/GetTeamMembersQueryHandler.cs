namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeamMembers;

/// <summary>
/// Handler for GetTeamMembersQuery that retrieves team members using repository pattern.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class GetTeamMembersQueryHandler : IRequestHandler<GetTeamMembersQuery, TeamMembersResponse>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMapper _mapper;

    public GetTeamMembersQueryHandler(
        ITeamRepository teamRepository,
        IMapper mapper)
    {
        _teamRepository = teamRepository;
        _mapper = mapper;
    }

    public async Task<TeamMembersResponse> Handle(GetTeamMembersQuery request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetWithMembersAsync(request.TeamId, cancellationToken).ConfigureAwait(false);
        if (team == null)
            throw new InvalidOperationException($"Team with ID '{request.TeamId}' not found");

        return _mapper.Map<TeamMembersResponse>(team);
    }
}
