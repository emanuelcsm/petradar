using MediatR;
using PetRadar.SharedKernel.Events;

namespace PetRadar.API.Infrastructure.Events;

internal sealed class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public MediatRDomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task DispatchAsync(DomainEvent domainEvent, CancellationToken ct = default)
        => _mediator.Publish(domainEvent, ct);

    public async Task DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken ct = default)
    {
        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent, ct);
    }
}
