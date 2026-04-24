using Animals.Domain.Events;
using MediatR;
using PetRadar.IntegrationEvents.Animals;

namespace Animals.Application.Integration.Publishers;

public sealed class AnimalPostDeletedIntegrationEventPublisher : INotificationHandler<AnimalPostDeletedDomainEvent>
{
    private readonly IMediator _mediator;

    public AnimalPostDeletedIntegrationEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AnimalPostDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new AnimalPostDeletedIntegrationEvent(
            notification.AnimalPostId,
            notification.UserId,
            notification.MediaIds,
            notification.Latitude,
            notification.Longitude,
            notification.DeletedAt);

        await _mediator.Publish(integrationEvent, cancellationToken);
    }
}
