namespace DotNetSkills.Domain.Common;

public abstract class BaseEntity<TId> : IEquatable<BaseEntity<TId>>
    where TId : IStronglyTypedId<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public static DateTime GetCurrentTime()
    {
        return DateTimeService.UtcNow;
    }

    protected BaseEntity(TId id)
    {
        Id = id;
        CreatedAt = DateTimeService.UtcNow;
        UpdatedAt = DateTimeService.UtcNow;
    }

    protected BaseEntity() 
    {
        CreatedAt = DateTimeService.UtcNow;
        UpdatedAt = DateTimeService.UtcNow;
    }

    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTimeService.UtcNow;
    }

    public virtual bool Equals(BaseEntity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as BaseEntity<TId>);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return !Equals(left, right);
    }
}