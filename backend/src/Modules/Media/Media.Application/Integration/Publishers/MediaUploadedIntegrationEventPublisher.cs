using Media.Domain.Events;
using Media.Application.Interfaces.Storage;
using MediatR;
using PetRadar.IntegrationEvents.Media;

namespace Media.Application.Integration.Publishers;

public sealed class MediaUploadedIntegrationEventPublisher : INotificationHandler<MediaUploadedDomainEvent>
{
    private readonly IMediator _mediator;
    private readonly IMediaStorage _mediaStorage;

    public MediaUploadedIntegrationEventPublisher(
        IMediator mediator,
        IMediaStorage mediaStorage)
    {
        _mediator = mediator;
        _mediaStorage = mediaStorage;
    }

    public async Task Handle(MediaUploadedDomainEvent notification, CancellationToken cancellationToken)
    {
        var publicUrl = _mediaStorage.GetUrl(notification.StoragePath);

        var integrationEvent = new MediaUploadedIntegrationEvent(
            notification.MediaId,
            publicUrl,
            notification.StoragePath,
            notification.UploadedBy);

        await _mediator.Publish(integrationEvent, cancellationToken);
    }
}
