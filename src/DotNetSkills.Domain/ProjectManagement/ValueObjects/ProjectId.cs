namespace DotNetSkills.Domain.ProjectManagement.ValueObjects;

/// <summary>
/// Represents a strongly-typed identifier for a Project entity.
/// </summary>
/// <param name="Value">The unique identifier value.</param>
public record ProjectId(Guid Value) : IStronglyTypedId<Guid>
{
    /// <summary>
    /// Creates a new unique ProjectId.
    /// </summary>
    /// <returns>A new ProjectId with a unique GUID value.</returns>
    public static ProjectId New() => new(Guid.NewGuid());

    /// <summary>
    /// Implicitly converts a ProjectId to its underlying Guid value.
    /// </summary>
    /// <param name="id">The ProjectId to convert.</param>
    public static implicit operator Guid(ProjectId id) => id.Value;

    /// <summary>
    /// Explicitly converts a Guid to a ProjectId.
    /// </summary>
    /// <param name="value">The Guid value to convert.</param>
    public static explicit operator ProjectId(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the ProjectId.
    /// </summary>
    /// <returns>The string representation of the underlying Guid.</returns>
    public override string ToString() => Value.ToString();
}
