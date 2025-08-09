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
        // Common value object & strongly-typed ID mappings moved to SharedValueObjectMappingProfile
        // to prevent duplicate type map registration across multiple profiles.
        ConfigureNullHandling();
    }

    /// <summary>
    /// Creates common value object mappings that are shared across all domain contexts.
    /// </summary>
    // Removed: common value object mappings now centralized in SharedValueObjectMappingProfile

    /// <summary>
    /// Creates strongly-typed ID to Guid mappings for all domain entities.
    /// This ensures consistent conversion patterns across all bounded contexts.
    /// </summary>
    // Removed: strongly-typed ID mappings now centralized in SharedValueObjectMappingProfile

    /// <summary>
    /// Configures null handling and conditional mapping patterns.
    /// Sets up default behaviors for nullable properties and collections.
    /// </summary>
    private void ConfigureNullHandling()
    {
        // Configure default null value handling
        AllowNullDestinationValues = true;
        AllowNullCollections = true;
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