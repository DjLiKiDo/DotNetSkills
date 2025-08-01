namespace DotNetSkills.Domain.ValueObjects;

/// <summary>
/// Represents a strongly-typed identifier for a Team entity.
/// </summary>
/// <param name="Value">The unique identifier value.</param>
public record TeamId(Guid Value) : IStronglyTypedId<Guid>
{
    /// <summary>
    /// Creates a new unique TeamId.
    /// </summary>
    /// <returns>A new TeamId with a unique GUID value.</returns>
    public static TeamId New() => new(Guid.NewGuid());

    /// <summary>
    /// Implicitly converts a TeamId to its underlying Guid value.
    /// </summary>
    /// <param name="id">The TeamId to convert.</param>
    public static implicit operator Guid(TeamId id) => id.Value;

    /// <summary>
    /// Explicitly converts a Guid to a TeamId.
    /// </summary>
    /// <param name="value">The Guid value to convert.</param>
    public static explicit operator TeamId(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the TeamId.
    /// </summary>
    /// <returns>The string representation of the underlying Guid.</returns>
    public override string ToString() => Value.ToString();
}
