namespace PetRadar.SharedKernel.Events;

/// <summary>
/// Dispatches domain events after aggregate persistence is confirmed.
/// Implementation lives in Infrastructure (MediatR). Application only uses this interface.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(DomainEvent domainEvent, CancellationToken ct = default);
    Task DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken ct = default);
}
