using MediatR;
using Media.Application.Interfaces.Persistence;
using Media.Application.Interfaces.Storage;
using PetRadar.IntegrationEvents.Animals;

namespace Media.Application.Integration;

public sealed class AnimalPostDeletedIntegrationEventHandler : INotificationHandler<AnimalPostDeletedIntegrationEvent>
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IMediaStorage _mediaStorage;

    public AnimalPostDeletedIntegrationEventHandler(
        IMediaRepository mediaRepository,
        IMediaStorage mediaStorage)
    {
        _mediaRepository = mediaRepository;
        _mediaStorage = mediaStorage;
    }

    public async Task Handle(AnimalPostDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        foreach (var mediaId in notification.MediaIds)
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId, cancellationToken);
            if (mediaFile is null)
                continue;

            await _mediaStorage.DeleteAsync(mediaFile.StoragePath, cancellationToken);
            await _mediaRepository.DeleteAsync(mediaId, cancellationToken);
        }
    }
}
