using DotNetSkills.Application.Common.Mappings;
using DotNetSkills.Application.UserManagement.Features.CreateUser;
using DotNetSkills.Application.UserManagement.Features.UpdateUser;
using DotNetSkills.Application.UserManagement.Features.UpdateUserRole;
using DotNetSkills.Application.UserManagement.Contracts.Requests;
using DotNetSkills.Application.UserManagement.Contracts.Responses;

namespace DotNetSkills.Application.UserManagement.Mappings;

/// <summary>
/// AutoMapper profile for User Management bounded context.
/// Provides mappings between domain entities and DTOs with proper value object handling.
/// </summary>
public class UserMappingProfile : MappingProfile
{
    /// <summary>
    /// Initializes a new instance of the UserMappingProfile class.
    /// Sets up all mappings for User Management entities and DTOs.
    /// </summary>
    public UserMappingProfile()
    {
        CreateUserMappings();
        CreateTeamMembershipMappings();
        CreateCommandMappings();
        CreateEnumMappings();
        CreatePagedResponseMappings();
    }

    /// <summary>
    /// Creates mappings for User entity and related DTOs.
    /// Handles EmailAddress value object conversion and strongly-typed ID mappings.
    /// </summary>
    private void CreateUserMappings()
    {
        // User to UserResponse mapping
        CreateMap<User, UserResponse>()
            .ConstructUsing(user => new UserResponse(
                user.Id.Value,
                user.Name,
                user.Email.Value,
                user.Role,
                user.Status,
                user.CreatedAt,
                user.UpdatedAt,
                user.TeamMemberships.Count))
            .ForAllMembers(opt => opt.Ignore()); // Use constructor mapping only

        // User to UserSummaryResponse (lightweight DTO for lists)
        CreateMap<User, UserSummaryResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        // User to UserProfileResponse (for profile management)
        CreateMap<User, UserProfileResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.CanBeAssignedTasks, opt => opt.MapFrom(src => src.CanBeAssignedTasks()))
            .ForMember(dest => dest.CanManageProjects, opt => opt.MapFrom(src => src.CanManageProjects()))
            .ForMember(dest => dest.CanManageTeams, opt => opt.MapFrom(src => src.CanManageTeams()));
    }

    /// <summary>
    /// Creates mappings for TeamMember entity and related DTOs.
    /// Handles team membership collection mappings with proper navigation.
    /// </summary>
    private void CreateTeamMembershipMappings()
    {
        // TeamMember to TeamMembershipResponse
        CreateMap<TeamMember, TeamMembershipResponse>()
            .ForMember(dest => dest.TeamMemberId, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.TeamId.Value))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.Value))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.JoinedAt))
            .ForMember(dest => dest.HasLeadershipPrivileges, opt => opt.MapFrom(src => src.HasLeadershipPrivileges()))
            .ForMember(dest => dest.CanBeAssignedTasks, opt => opt.MapFrom(src => src.CanBeAssignedTasks()))
            .ForMember(dest => dest.CanAssignTasks, opt => opt.MapFrom(src => src.CanAssignTasks()))
            // These will be populated by custom resolvers or separate queries
            .ForMember(dest => dest.TeamName, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.Ignore());

        // TeamMember to TeamMembershipDto (for nested DTOs)
        CreateMap<TeamMember, TeamMembershipDto>()
            .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.TeamId))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.JoinedAt))
            .ForMember(dest => dest.TeamName, opt => opt.Ignore()); // Will be populated by custom logic

        // User to TeamMembershipListDto (for getting user's team memberships)
        CreateMap<User, TeamMembershipListDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TeamMemberships, opt => opt.MapFrom(src => src.TeamMemberships));
    }

    /// <summary>
    /// Creates reverse mappings for command operations.
    /// Maps DTOs to domain commands for create and update operations.
    /// Note: Command properties will be updated when handlers are implemented.
    /// </summary>
    private void CreateCommandMappings()
    {
        // CreateUserRequest to CreateUserCommand (using actual command structure)
        CreateMap<CreateUserRequest, CreateUserCommand>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

        // UpdateUserRequest to UpdateUserCommand (basic mapping)
        CreateMap<UpdateUserRequest, UpdateUserCommand>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        // UpdateUserRoleRequest to UpdateUserRoleCommand (basic mapping)
        CreateMap<UpdateUserRoleRequest, UpdateUserRoleCommand>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
    }

    /// <summary>
    /// Creates enum mappings with proper string conversion.
    /// </summary>
    private void CreateEnumMappings()
    {
        // API uses JsonStringEnumConverter, so enums will serialize as strings automatically.
        // No custom enum-to-string mapping is needed for response DTOs now that they use enum types.
    }

    /// <summary>
    /// Creates paginated response mappings for user collections.
    /// </summary>
    private void CreatePagedResponseMappings()
    {
        CreatePagedMapping<User, UserResponse>();
        CreatePagedMapping<User, UserSummaryResponse>();

        // Specific mapping for PagedUserResponse using correct property names
        CreateMap<PagedResponse<User>, PagedUserResponse>()
            .ConvertUsing((src, dest, context) => new PagedUserResponse(
                Users: context.Mapper.Map<List<UserResponse>>(src.Data).AsReadOnly(),
                TotalCount: src.TotalCount,
                Page: src.PageNumber,
                PageSize: src.PageSize));
    }

    // Custom value resolvers would be implemented here when Infrastructure layer is ready
    // For now, team names and other navigation properties will be handled by separate queries
}

/// <summary>
/// Additional DTO classes that may not exist yet but are referenced in the mappings.
/// These follow the established patterns and will be created as needed.
/// </summary>

/// <summary>
/// Lightweight DTO for user summaries in lists and dropdowns.
/// </summary>
public record UserSummaryResponse(
    Guid Id,
    string Name,
    string Email,
    UserRole Role,
    UserStatus Status);

/// <summary>
/// Detailed DTO for user profile management operations.
/// </summary>
public record UserProfileResponse(
    Guid Id,
    string Name,
    string Email,
    UserRole Role,
    UserStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool CanBeAssignedTasks,
    bool CanManageProjects,
    bool CanManageTeams);

/// <summary>
/// DTO for team membership information with enhanced metadata.
/// </summary>
public record TeamMembershipResponse(
    Guid TeamMemberId,
    Guid TeamId,
    string TeamName,
    Guid UserId,
    string UserName,
    TeamRole Role,
    DateTime JoinedAt,
    bool HasLeadershipPrivileges,
    bool CanBeAssignedTasks,
    bool CanAssignTasks);