using Media.Domain.Events;
using MediatR;
using PetRadar.IntegrationEvents.Media;

namespace Media.Application.Integration.Publishers;

public sealed class MediaUploadedIntegrationEventPublisher : INotificationHandler<MediaUploadedDomainEvent>
{
    private readonly IMediator _mediator;

    public MediaUploadedIntegrationEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(MediaUploadedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new MediaUploadedIntegrationEvent(
            notification.MediaId,
            notification.StoragePath,
            notification.UploadedBy);

        await _mediator.Publish(integrationEvent, cancellationToken);
    }
}
