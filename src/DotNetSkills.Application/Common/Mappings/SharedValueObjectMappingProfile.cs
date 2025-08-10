using AutoMapper;

namespace DotNetSkills.Application.Common.Mappings;

/// <summary>
/// Centralized profile for value object and strongly-typed ID conversions shared across all other profiles.
/// Having these in one profile prevents DuplicateTypeMapConfigurationException.
/// </summary>
public sealed class SharedValueObjectMappingProfile : Profile
{
    public SharedValueObjectMappingProfile()
    {
        CreateCommonValueObjectMappings();
        CreateStronglyTypedIdMappings();
    CreateCollectionMappings();
    }

    private void CreateCommonValueObjectMappings()
    {
        CreateMap<EmailAddress, string>().ConvertUsing(email => email.Value);
        CreateMap<string, EmailAddress>().ConvertUsing(email => new EmailAddress(email));

        CreateMap<DateTime, DateTime>().ConvertUsing(dt => DateTime.SpecifyKind(dt, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(dt => dt.HasValue ? DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc) : null);
    }

    private void CreateStronglyTypedIdMappings()
    {
        CreateMap<UserId, Guid>().ConvertUsing(id => id.Value);
        CreateMap<Guid, UserId>().ConvertUsing(g => new UserId(g));

        CreateMap<TeamId, Guid>().ConvertUsing(id => id.Value);
        CreateMap<Guid, TeamId>().ConvertUsing(g => new TeamId(g));

        CreateMap<TeamMemberId, Guid>().ConvertUsing(id => id.Value);
        CreateMap<Guid, TeamMemberId>().ConvertUsing(g => new TeamMemberId(g));

        CreateMap<ProjectId, Guid>().ConvertUsing(id => id.Value);
        CreateMap<Guid, ProjectId>().ConvertUsing(g => new ProjectId(g));

        CreateMap<TaskId, Guid>().ConvertUsing(id => id.Value);
        CreateMap<Guid, TaskId>().ConvertUsing(g => new TaskId(g));
    }

    private void CreateCollectionMappings()
    {
        // Single registration for generic object collection conversion
        CreateMap<IEnumerable<object>, List<object>>()
            .ConvertUsing((src, dest, context) => src?.ToList() ?? new List<object>());
    }
}
