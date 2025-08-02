namespace DotNetSkills.Application.TeamCollaboration.Queries;

/// <summary>
/// Query for retrieving all members of a team.
/// </summary>
public record GetTeamMembersQuery(TeamId TeamId) : IRequest<TeamMembersResponse>;

public class GetTeamMembersQueryHandler : IRequestHandler<GetTeamMembersQuery, TeamMembersResponse>
{
    public async Task<TeamMembersResponse> Handle(GetTeamMembersQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement team members retrieval logic
        // 1. Get team from repository by ID with members included
        // 2. Map to TeamMembersResponse DTO
        // 3. Return response with member details
        
        await Task.CompletedTask;
        throw new NotImplementedException("GetTeamMembersQueryHandler requires Infrastructure layer implementation");
    }
}