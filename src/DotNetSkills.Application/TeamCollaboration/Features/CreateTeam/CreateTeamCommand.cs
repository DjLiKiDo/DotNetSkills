namespace DotNetSkills.Application.TeamCollaboration.Features.CreateTeam;

/// <summary>
/// Command for creating a new team.
/// </summary>
public record CreateTeamCommand(string Name, string? Description) : IRequest<TeamResponse>;
