using Animals.Domain.Events;
using MediatR;
using PetRadar.IntegrationEvents.Animals;

namespace Animals.Application.Integration.Publishers;

public sealed class AnimalPostedIntegrationEventPublisher : INotificationHandler<AnimalPostedEvent>
{
    private readonly IMediator _mediator;

    public AnimalPostedIntegrationEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AnimalPostedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new AnimalPostedIntegrationEvent(
            notification.AnimalPostId,
            notification.UserId,
            notification.Latitude,
            notification.Longitude,
            notification.CreatedAt);

        await _mediator.Publish(integrationEvent, cancellationToken);
    }
}
