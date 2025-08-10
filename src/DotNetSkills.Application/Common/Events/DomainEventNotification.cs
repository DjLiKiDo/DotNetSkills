namespace DotNetSkills.Application.Common.Events;

/// <summary>
/// Wrapper that adapts IDomainEvent to MediatR's INotification interface.
/// This maintains the separation between Domain and Infrastructure concerns
/// while enabling domain events to be processed through MediatR.
/// </summary>
/// <typeparam name="TDomainEvent">The type of domain event being wrapped.</typeparam>
public class DomainEventNotification<TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the wrapped domain event.
    /// </summary>
    public TDomainEvent DomainEvent { get; }

    /// <summary>
    /// Initializes a new instance of the DomainEventNotification class.
    /// </summary>
    /// <param name="domainEvent">The domain event to wrap.</param>
    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent ?? throw new ArgumentNullException(nameof(domainEvent));
    }
}
