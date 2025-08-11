namespace DotNetSkills.Domain.Common.Entities;

/// <summary>
/// Non-generic interface for aggregate roots to enable polymorphic handling.
/// This interface allows collections of different aggregate root types to be handled uniformly
/// for operations like domain event collection and dispatching.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Gets the domain events that have been raised by this aggregate root.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all domain events from this aggregate root.
    /// This should be called after successful domain event dispatching.
    /// </summary>
    void ClearDomainEvents();

    /// <summary>
    /// Gets the unique identifier of this aggregate root as a Guid.
    /// This provides a common way to identify aggregates across different types.
    /// </summary>
    Guid GetId();

    /// <summary>
    /// Gets the type name of this aggregate root.
    /// This is useful for logging and debugging purposes.
    /// </summary>
    string GetTypeName();
}
