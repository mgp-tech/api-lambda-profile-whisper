namespace Profile.Core.Domain.Entity;

public abstract class BaseEntity
{
    protected BaseEntity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; protected init; }
}