namespace DotNetSkills.Domain.ValueObjects;

/// <summary>
/// Represents a strongly-typed identifier for a Task entity.
/// </summary>
/// <param name="Value">The unique identifier value.</param>
public record TaskId(Guid Value) : IStronglyTypedId<Guid>
{
    /// <summary>
    /// Creates a new unique TaskId.
    /// </summary>
    /// <returns>A new TaskId with a unique GUID value.</returns>
    public static TaskId New() => new(Guid.NewGuid());

    /// <summary>
    /// Implicitly converts a TaskId to its underlying Guid value.
    /// </summary>
    /// <param name="id">The TaskId to convert.</param>
    public static implicit operator Guid(TaskId id) => id.Value;

    /// <summary>
    /// Explicitly converts a Guid to a TaskId.
    /// </summary>
    /// <param name="value">The Guid value to convert.</param>
    public static explicit operator TaskId(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the TaskId.
    /// </summary>
    /// <returns>The string representation of the underlying Guid.</returns>
    public override string ToString() => Value.ToString();
}
