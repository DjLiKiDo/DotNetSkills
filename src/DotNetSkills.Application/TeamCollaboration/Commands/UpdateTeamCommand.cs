namespace DotNetSkills.Application.TeamCollaboration.Commands;

/// <summary>
/// Command for updating an existing team.
/// </summary>
public record UpdateTeamCommand(TeamId TeamId, string Name, string? Description) : IRequest<TeamResponse>;

public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, TeamResponse>
{
    public async Task<TeamResponse> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement team update logic
        // 1. Get team from repository by ID
        // 2. Validate user has permission to update team
        // 3. Call team.UpdateInfo() domain method
        // 4. Save changes to repository
        // 5. Map to DTO and return
        
        await Task.CompletedTask;
        throw new NotImplementedException("UpdateTeamCommandHandler requires Infrastructure layer implementation");
    }
}