using System.Linq.Expressions;

namespace DotNetSkills.Application.Common.Mappings;

/// <summary>
/// Base AutoMapper profile that provides common mapping configurations and value object conversions.
/// This profile is inherited by specific domain mapping profiles to ensure consistent mapping patterns.
/// </summary>
public abstract class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the MappingProfile class.
    /// Sets up common mapping configurations including strongly-typed ID conversions,
    /// value object mappings, and null handling patterns.
    /// </summary>
    protected MappingProfile()
    {
        CreateCommonValueObjectMappings();
        CreateStronglyTypedIdMappings();
        ConfigureNullHandling();
    }

    /// <summary>
    /// Creates common value object mappings that are shared across all domain contexts.
    /// </summary>
    private void CreateCommonValueObjectMappings()
    {
        // EmailAddress value object mappings
        CreateMap<EmailAddress, string>()
            .ConvertUsing(email => email.Value);

        CreateMap<string, EmailAddress>()
            .ConvertUsing(email => new EmailAddress(email));

        // DateTime mappings with UTC handling
        CreateMap<DateTime, DateTime>()
            .ConvertUsing(dt => DateTime.SpecifyKind(dt, DateTimeKind.Utc));

        CreateMap<DateTime?, DateTime?>()
            .ConvertUsing(dt => dt.HasValue ? DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc) : null);
    }

    /// <summary>
    /// Creates strongly-typed ID to Guid mappings for all domain entities.
    /// This ensures consistent conversion patterns across all bounded contexts.
    /// </summary>
    private void CreateStronglyTypedIdMappings()
    {
        // User Management strongly-typed IDs
        CreateMap<UserId, Guid>()
            .ConvertUsing(id => id.Value);
        CreateMap<Guid, UserId>()
            .ConvertUsing(guid => new UserId(guid));

        // Team Collaboration strongly-typed IDs
        CreateMap<TeamId, Guid>()
            .ConvertUsing(id => id.Value);
        CreateMap<Guid, TeamId>()
            .ConvertUsing(guid => new TeamId(guid));

        CreateMap<TeamMemberId, Guid>()
            .ConvertUsing(id => id.Value);
        CreateMap<Guid, TeamMemberId>()
            .ConvertUsing(guid => new TeamMemberId(guid));

        // Project Management strongly-typed IDs
        CreateMap<ProjectId, Guid>()
            .ConvertUsing(id => id.Value);
        CreateMap<Guid, ProjectId>()
            .ConvertUsing(guid => new ProjectId(guid));

        // Task Execution strongly-typed IDs
        CreateMap<TaskId, Guid>()
            .ConvertUsing(id => id.Value);
        CreateMap<Guid, TaskId>()
            .ConvertUsing(guid => new TaskId(guid));
    }

    /// <summary>
    /// Configures null handling and conditional mapping patterns.
    /// Sets up default behaviors for nullable properties and collections.
    /// </summary>
    private void ConfigureNullHandling()
    {
        // Configure default null value handling
        AllowNullDestinationValues = true;
        AllowNullCollections = true;

        // Configure collection handling - empty collections instead of null
        CreateMap<IEnumerable<object>, List<object>>()
            .ConvertUsing((src, dest, context) => src?.ToList() ?? new List<object>());
    }

    /// <summary>
    /// Helper method to create enum to string mappings with proper null handling.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to map.</typeparam>
    protected void CreateEnumToStringMapping<TEnum>() where TEnum : struct, Enum
    {
        CreateMap<TEnum, string>()
            .ConvertUsing(enumValue => enumValue.ToString());

        CreateMap<TEnum?, string?>()
            .ConvertUsing(enumValue => enumValue.HasValue ? enumValue.Value.ToString() : null);

        CreateMap<string, TEnum>()
            .ConvertUsing(str => Enum.Parse<TEnum>(str, true));

        CreateMap<string?, TEnum?>()
            .ConvertUsing(str => string.IsNullOrEmpty(str) ? (TEnum?)null : Enum.Parse<TEnum>(str, true));
    }

    /// <summary>
    /// Helper method to create paginated response mappings.
    /// </summary>
    /// <typeparam name="TSource">The source item type.</typeparam>
    /// <typeparam name="TDestination">The destination item type.</typeparam>
    protected void CreatePagedMapping<TSource, TDestination>()
    {
        CreateMap<PagedResponse<TSource>, PagedResponse<TDestination>>()
            .ConvertUsing((src, dest, context) => new PagedResponse<TDestination>(
                data: context.Mapper.Map<List<TDestination>>(src.Data),
                pageNumber: src.PageNumber,
                pageSize: src.PageSize,
                totalCount: src.TotalCount));
    }

    /// <summary>
    /// Helper method to create mappings with reverse mappings that ignore specific properties.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    protected IMappingExpression<TSource, TDestination> CreateMappingWithIgnoredReverseProperties<TSource, TDestination>()
    {
        return CreateMap<TSource, TDestination>();
    }
}