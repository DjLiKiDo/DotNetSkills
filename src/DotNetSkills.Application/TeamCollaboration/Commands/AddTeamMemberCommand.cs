namespace DotNetSkills.Application.TeamCollaboration.Commands;

/// <summary>
/// Command for adding a member to a team.
/// Enforces Team.MaxMembers constraint and prevents duplicate memberships.
/// </summary>
public record AddTeamMemberCommand(TeamId TeamId, UserId UserId, TeamRole Role) : IRequest<TeamMemberResponse>;

public class AddTeamMemberCommandHandler : IRequestHandler<AddTeamMemberCommand, TeamMemberResponse>
{
    public async Task<TeamMemberResponse> Handle(AddTeamMemberCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement team member addition logic
        // 1. Get team from repository by ID
        // 2. Get user from repository by ID
        // 3. Get current user (addedBy) from context
        // 4. Call team.AddMember() domain method (enforces business rules)
        // 5. Save changes to repository
        // 6. Map to TeamMemberResponse DTO and return

        await Task.CompletedTask;
        throw new NotImplementedException("AddTeamMemberCommandHandler requires Infrastructure layer implementation");
    }
}
