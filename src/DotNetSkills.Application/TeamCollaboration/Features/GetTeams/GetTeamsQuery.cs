namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeams;

/// <summary>
/// Query for retrieving teams with pagination and optional search.
/// </summary>
public record GetTeamsQuery(int Page, int PageSize, string? Search) : IRequest<PagedTeamResponse>;
