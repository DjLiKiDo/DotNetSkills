namespace DotNetSkills.Domain.ValueObjects;

/// <summary>
/// Represents a strongly-typed identifier for a TeamMember entity.
/// </summary>
/// <param name="Value">The unique identifier value.</param>
public record TeamMemberId(Guid Value) : IStronglyTypedId<Guid>
{
    /// <summary>
    /// Creates a new unique TeamMemberId.
    /// </summary>
    /// <returns>A new TeamMemberId with a unique GUID value.</returns>
    public static TeamMemberId New() => new(Guid.NewGuid());

    /// <summary>
    /// Implicitly converts a TeamMemberId to its underlying Guid value.
    /// </summary>
    /// <param name="id">The TeamMemberId to convert.</param>
    public static implicit operator Guid(TeamMemberId id) => id.Value;

    /// <summary>
    /// Explicitly converts a Guid to a TeamMemberId.
    /// </summary>
    /// <param name="value">The Guid value to convert.</param>
    public static explicit operator TeamMemberId(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the TeamMemberId.
    /// </summary>
    /// <returns>The string representation of the underlying Guid.</returns>
    public override string ToString() => Value.ToString();
}
