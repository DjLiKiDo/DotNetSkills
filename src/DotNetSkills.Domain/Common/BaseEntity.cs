namespace DotNetSkills.Domain.Common;

public abstract class BaseEntity<TId> where TId : IStronglyTypedId<Guid>
{
    public TId Id { get; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public Guid? CreatedBy { get; private set; }

    public Guid? UpdatedBy { get; private set; }

    protected BaseEntity(TId id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        var utcNow = DateTime.UtcNow;
        CreatedAt = utcNow;
        UpdatedAt = utcNow;
    }

    protected internal virtual void SetCreatedBy(Guid? createdBy)
    {
        CreatedBy = createdBy;
    }

    protected internal virtual void SetUpdatedBy(Guid? updatedBy)
    {
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
