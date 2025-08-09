namespace DotNetSkills.Application.UserManagement.Features.GetUserTeamMemberships;

/// <summary>
/// Query to retrieve all team memberships for a specific user.
/// Returns a list of teams the user belongs to along with their roles and membership details.
/// </summary>
/// <param name="UserId">The unique identifier of the user whose team memberships to retrieve.</param>
public record GetUserTeamMembershipsQuery(UserId UserId) : IRequest<Result<TeamMembershipListDto>>;
