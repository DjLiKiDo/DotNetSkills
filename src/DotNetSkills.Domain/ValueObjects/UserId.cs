namespace DotNetSkills.Domain.ValueObjects;

/// <summary>
/// Represents a strongly-typed identifier for a User entity.
/// </summary>
/// <param name="Value">The unique identifier value.</param>
public record UserId(Guid Value) : IStronglyTypedId<Guid>
{
    /// <summary>
    /// Creates a new unique UserId.
    /// </summary>
    /// <returns>A new UserId with a unique GUID value.</returns>
    public static UserId New() => new(Guid.NewGuid());

    /// <summary>
    /// Implicitly converts a UserId to its underlying Guid value.
    /// </summary>
    /// <param name="id">The UserId to convert.</param>
    public static implicit operator Guid(UserId id) => id.Value;

    /// <summary>
    /// Explicitly converts a Guid to a UserId.
    /// </summary>
    /// <param name="value">The Guid value to convert.</param>
    public static explicit operator UserId(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the UserId.
    /// </summary>
    /// <returns>The string representation of the underlying Guid.</returns>
    public override string ToString() => Value.ToString();
}
