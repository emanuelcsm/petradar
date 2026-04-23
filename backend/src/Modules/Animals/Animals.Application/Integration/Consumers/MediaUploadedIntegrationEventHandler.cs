using Animals.Application.Integration.Interfaces;
using MediatR;
using PetRadar.IntegrationEvents.Media;

namespace Animals.Application.Integration.Consumers;

public sealed class MediaUploadedIntegrationEventHandler : INotificationHandler<MediaUploadedIntegrationEvent>
{
    private readonly IAnimalPostMediaRepository _animalPostMediaRepository;

    public MediaUploadedIntegrationEventHandler(IAnimalPostMediaRepository animalPostMediaRepository)
    {
        _animalPostMediaRepository = animalPostMediaRepository;
    }

    public Task Handle(MediaUploadedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        return _animalPostMediaRepository.SaveAsync(
            notification.MediaId,
            notification.StoragePath,
            cancellationToken);
    }
}
