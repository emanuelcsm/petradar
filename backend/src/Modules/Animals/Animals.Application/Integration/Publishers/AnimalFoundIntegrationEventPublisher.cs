using Animals.Domain.Events;
using MediatR;
using PetRadar.IntegrationEvents.Animals;

namespace Animals.Application.Integration.Publishers;

public sealed class AnimalFoundIntegrationEventPublisher : INotificationHandler<AnimalFoundEvent>
{
    private readonly IMediator _mediator;

    public AnimalFoundIntegrationEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AnimalFoundEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new AnimalFoundIntegrationEvent(
            notification.AnimalPostId,
            notification.UserId,
            notification.FoundAt);

        await _mediator.Publish(integrationEvent, cancellationToken);
    }
}
