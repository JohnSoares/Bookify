namespace Bookify.Domain.Abstractions;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyList<IDomainEvent> DomainEvents => [.. _domainEvents];

    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity(){ }

    public Guid Id { get; init; }

    public void ClearDomainEvents() 
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent) 
    {
        _domainEvents.Add(domainEvent);
    }
}
