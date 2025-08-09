namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeam;

/// <summary>
/// Handler for GetTeamByIdQuery that retrieves team details using repository pattern.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class GetTeamByIdQueryHandler : IRequestHandler<GetTeamByIdQuery, TeamResponse>
{
    private readonly ITeamRepository _teamRepository;

    public GetTeamByIdQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<TeamResponse> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetWithMembersAsync(request.TeamId, cancellationToken).ConfigureAwait(false);
        if (team == null)
            throw new InvalidOperationException($"Team with ID '{request.TeamId}' not found");

        return TeamResponse.FromDomain(team);
    }
}
