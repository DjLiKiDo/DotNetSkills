namespace DotNetSkills.Application.UserManagement.Contracts.Responses;

public record TeamMembershipListDto
{
    public required UserId UserId { get; init; }
    public required string UserName { get; init; }
    public required IReadOnlyList<TeamMembershipDto> TeamMemberships { get; init; }
}

public record TeamMembershipDto
{
    public required TeamId TeamId { get; init; }
    public required string TeamName { get; init; }
    public required TeamRole Role { get; init; }
    public required DateTime JoinedAt { get; init; }
}
