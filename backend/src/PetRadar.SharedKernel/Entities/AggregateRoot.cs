using PetRadar.SharedKernel.Events;

namespace PetRadar.SharedKernel.Entities;

public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = [];

    protected AggregateRoot() { }

    protected AggregateRoot(string id) : base(id) { }

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Returns a snapshot of all accumulated domain events and clears the internal list.
    /// Call this after a successful SaveAsync, then dispatch the returned events.
    /// </summary>
    public IReadOnlyList<DomainEvent> CollectDomainEvents()
    {
        var snapshot = _domainEvents.ToList();
        _domainEvents.Clear();
        return snapshot.AsReadOnly();
    }
}
