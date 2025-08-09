namespace DotNetSkills.Infrastructure.Persistence;

/// <summary>
/// Static utility class containing value converters for strongly-typed IDs used in Entity Framework Core.
/// Provides conversion patterns for mapping strongly-typed domain IDs to database primitive types.
/// </summary>
public static class ValueConverters
{
    /// <summary>
    /// Creates a value converter for strongly-typed IDs that implement IStronglyTypedId{Guid}.
    /// Converts from the strongly-typed ID to Guid for database storage and back.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed ID type.</typeparam>
    /// <param name="createIdFromGuid">Factory function to create the ID from a Guid value.</param>
    /// <returns>A ValueConverter for the strongly-typed ID.</returns>
    public static ValueConverter<TId, Guid> CreateStronglyTypedIdConverter<TId>(
        Func<Guid, TId> createIdFromGuid)
        where TId : IStronglyTypedId<Guid>
    {
        return new ValueConverter<TId, Guid>(
            id => id.Value,
            value => createIdFromGuid(value));
    }
    
    /// <summary>
    /// Creates a value converter for nullable strongly-typed IDs that implement IStronglyTypedId{Guid}.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed ID type.</typeparam>
    /// <param name="createIdFromGuid">Factory function to create the ID from a Guid value.</param>
    /// <returns>A ValueConverter for the nullable strongly-typed ID.</returns>
    public static ValueConverter<TId?, Guid?> CreateNullableStronglyTypedIdConverter<TId>(
        Func<Guid, TId> createIdFromGuid)
        where TId : IStronglyTypedId<Guid>
    {
        return new ValueConverter<TId?, Guid?>(
            id => id != null ? id.Value : null,
            value => value.HasValue ? createIdFromGuid(value.Value) : default(TId?));
    }
    
    /// <summary>
    /// Creates a value converter for UserId specifically.
    /// </summary>
    /// <returns>A ValueConverter for UserId.</returns>
    public static ValueConverter<UserId, Guid> CreateUserIdConverter()
    {
        return CreateStronglyTypedIdConverter<UserId>(value => new UserId(value));
    }
    
    /// <summary>
    /// Creates a value converter for nullable UserId specifically.
    /// </summary>
    /// <returns>A ValueConverter for nullable UserId.</returns>
    public static ValueConverter<UserId?, Guid?> CreateNullableUserIdConverter()
    {
        return CreateNullableStronglyTypedIdConverter<UserId>(value => new UserId(value));
    }
    
    /// <summary>
    /// Creates a value converter for TeamId specifically.
    /// </summary>
    /// <returns>A ValueConverter for TeamId.</returns>
    public static ValueConverter<TeamId, Guid> CreateTeamIdConverter()
    {
        return CreateStronglyTypedIdConverter<TeamId>(value => new TeamId(value));
    }
    
    /// <summary>
    /// Creates a value converter for nullable TeamId specifically.
    /// </summary>
    /// <returns>A ValueConverter for nullable TeamId.</returns>
    public static ValueConverter<TeamId?, Guid?> CreateNullableTeamIdConverter()
    {
        return CreateNullableStronglyTypedIdConverter<TeamId>(value => new TeamId(value));
    }
    
    /// <summary>
    /// Creates a value converter for TeamMemberId specifically.
    /// </summary>
    /// <returns>A ValueConverter for TeamMemberId.</returns>
    public static ValueConverter<TeamMemberId, Guid> CreateTeamMemberIdConverter()
    {
        return CreateStronglyTypedIdConverter<TeamMemberId>(value => new TeamMemberId(value));
    }
    
    /// <summary>
    /// Creates a value converter for ProjectId specifically.
    /// </summary>
    /// <returns>A ValueConverter for ProjectId.</returns>
    public static ValueConverter<ProjectId, Guid> CreateProjectIdConverter()
    {
        return CreateStronglyTypedIdConverter<ProjectId>(value => new ProjectId(value));
    }
    
    /// <summary>
    /// Creates a value converter for nullable ProjectId specifically.
    /// </summary>
    /// <returns>A ValueConverter for nullable ProjectId.</returns>
    public static ValueConverter<ProjectId?, Guid?> CreateNullableProjectIdConverter()
    {
        return CreateNullableStronglyTypedIdConverter<ProjectId>(value => new ProjectId(value));
    }
    
    /// <summary>
    /// Creates a value converter for TaskId specifically.
    /// </summary>
    /// <returns>A ValueConverter for TaskId.</returns>
    public static ValueConverter<TaskId, Guid> CreateTaskIdConverter()
    {
        return CreateStronglyTypedIdConverter<TaskId>(value => new TaskId(value));
    }
    
    /// <summary>
    /// Creates a value converter for nullable TaskId specifically.
    /// </summary>
    /// <returns>A ValueConverter for nullable TaskId.</returns>
    public static ValueConverter<TaskId?, Guid?> CreateNullableTaskIdConverter()
    {
        return CreateNullableStronglyTypedIdConverter<TaskId>(value => new TaskId(value));
    }
}
