namespace DotNetSkills.Application.TeamCollaboration.Commands;

/// <summary>
/// Command for creating a new team.
/// </summary>
public record CreateTeamCommand(string Name, string? Description) : IRequest<TeamResponse>;

public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, TeamResponse>
{
    public async Task<TeamResponse> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement team creation logic
        // 1. Get current user from context (requires authentication)
        // 2. Validate user has permission to create teams
        // 3. Create team using Team.Create() domain method
        // 4. Save to repository
        // 5. Map to DTO and return

        await Task.CompletedTask;
        throw new NotImplementedException("CreateTeamCommandHandler requires Infrastructure layer implementation");
    }
}
