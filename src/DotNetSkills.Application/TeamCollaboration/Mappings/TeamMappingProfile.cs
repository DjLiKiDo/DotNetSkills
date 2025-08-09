using DotNetSkills.Application.Common.Mappings;
using DotNetSkills.Application.TeamCollaboration.Contracts.Responses;

namespace DotNetSkills.Application.TeamCollaboration.Mappings;

/// <summary>
/// AutoMapper profile for Team Collaboration bounded context.
/// Provides mappings between domain entities and DTOs with proper value object handling.
/// </summary>
public class TeamMappingProfile : MappingProfile
{
    /// <summary>
    /// Initializes a new instance of the TeamMappingProfile class.
    /// Sets up all mappings for Team Collaboration entities and DTOs.
    /// </summary>
    public TeamMappingProfile()
    {
        CreateTeamMappings();
        CreateTeamMemberMappings();
        CreatePagedResponseMappings();
    }

    /// <summary>
    /// Creates mappings for Team entity and related DTOs.
    /// Handles team data conversion and strongly-typed ID mappings.
    /// </summary>
    private void CreateTeamMappings()
    {
        // Team to TeamResponse mapping
        CreateMap<Team, TeamResponse>()
            .ConstructUsing(team => new TeamResponse(
                team.Id.Value,
                team.Name,
                team.Description,
                team.MemberCount,
                team.CreatedAt,
                team.UpdatedAt,
                team.Members.Select(member => new TeamMemberResponse(
                    member.UserId.Value,
                    member.Role,
                    member.JoinedAt)).ToList().AsReadOnly()))
            .ForAllMembers(opt => opt.Ignore()); // Use constructor mapping only

        // Team to TeamMembersResponse mapping
        CreateMap<Team, TeamMembersResponse>()
            .ConstructUsing(team => new TeamMembersResponse(
                team.Id.Value,
                team.Name,
                team.MemberCount,
                Team.MaxMembers,
                team.Members.Select(member => new TeamMemberResponse(
                    member.UserId.Value,
                    member.Role,
                    member.JoinedAt)).ToList().AsReadOnly()))
            .ForAllMembers(opt => opt.Ignore()); // Use constructor mapping only
    }

    /// <summary>
    /// Creates mappings for TeamMember entity and related DTOs.
    /// </summary>
    private void CreateTeamMemberMappings()
    {
        // TeamMember to TeamMemberResponse mapping
        CreateMap<TeamMember, TeamMemberResponse>()
            .ConstructUsing(member => new TeamMemberResponse(
                member.UserId.Value,
                member.Role,
                member.JoinedAt))
            .ForAllMembers(opt => opt.Ignore()); // Use constructor mapping only
    }

    /// <summary>
    /// Creates paginated response mappings for team collections.
    /// </summary>
    private void CreatePagedResponseMappings()
    {
        CreatePagedMapping<Team, TeamResponse>();

        // Specific mapping for PagedTeamResponse using correct property names
        CreateMap<PagedResponse<Team>, PagedTeamResponse>()
            .ConvertUsing((src, dest, context) => new PagedTeamResponse(
                Teams: context.Mapper.Map<List<TeamResponse>>(src.Data).AsReadOnly(),
                TotalCount: src.TotalCount,
                Page: src.PageNumber,
                PageSize: src.PageSize));
    }
}