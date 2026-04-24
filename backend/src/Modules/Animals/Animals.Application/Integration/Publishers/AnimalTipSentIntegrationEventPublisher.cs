using Animals.Domain.Events;
using MediatR;
using PetRadar.IntegrationEvents.Animals;

namespace Animals.Application.Integration.Publishers;

public sealed class AnimalTipSentIntegrationEventPublisher : INotificationHandler<AnimalTipSentDomainEvent>
{
    private readonly IMediator _mediator;

    public AnimalTipSentIntegrationEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AnimalTipSentDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new AnimalTipSentIntegrationEvent(
            notification.AnimalPostId,
            notification.OwnerId,
            notification.SenderName,
            notification.Message);

        await _mediator.Publish(integrationEvent, cancellationToken);
    }
}
