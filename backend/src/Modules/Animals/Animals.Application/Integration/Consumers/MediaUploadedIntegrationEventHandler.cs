using Animals.Application.Integration.Interfaces;
using MediatR;
using PetRadar.IntegrationEvents.Media;

namespace Animals.Application.Integration.Consumers;

public sealed class MediaUploadedIntegrationEventHandler : INotificationHandler<MediaUploadedIntegrationEvent>
{
    private readonly IKnownMediaRepository _knownMediaRepository;

    public MediaUploadedIntegrationEventHandler(IKnownMediaRepository knownMediaRepository)
    {
        _knownMediaRepository = knownMediaRepository;
    }

    public Task Handle(MediaUploadedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        return _knownMediaRepository.SaveAsync(
            notification.MediaId,
            notification.PublicUrl,
            notification.StoragePath,
            cancellationToken);
    }
}
