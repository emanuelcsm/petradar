using MediatR;
using Notifications.Application.Interfaces;
using PetRadar.IntegrationEvents.Animals;

namespace Notifications.Application.Integration.Consumers;

public sealed class AnimalPostedIntegrationEventHandler : INotificationHandler<AnimalPostedIntegrationEvent>
{
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;

    public AnimalPostedIntegrationEventHandler(
        INotificationService notificationService,
        INotificationRepository notificationRepository)
    {
        _notificationService = notificationService;
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(AnimalPostedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var regionKey = BuildRegionKey(notification.Latitude, notification.Longitude);
        const string eventName = "animal-posted";

        var payload = new
        {
            notification.AnimalPostId,
            notification.UserId,
            notification.Latitude,
            notification.Longitude,
            notification.CreatedAt
        };

        await _notificationService.BroadcastToRegionAsync(
            regionKey,
            eventName,
            payload,
            cancellationToken);

        var notificationRecord = new NotificationWriteModel(
            EventName: eventName,
            Message: "Novo animal reportado na sua regiao.",
            CreatedAt: notification.CreatedAt,
            RegionKey: regionKey,
            Payload: payload);

        await _notificationRepository.SaveAsync(notificationRecord, cancellationToken);
    }

    private static string BuildRegionKey(double latitude, double longitude)
    {
        // Usa grade simples e deterministica para agrupar coordenadas em uma mesma regiao.
        var latBucket = Math.Round(latitude, 1, MidpointRounding.AwayFromZero);
        var lngBucket = Math.Round(longitude, 1, MidpointRounding.AwayFromZero);

        return $"region:{latBucket:F1}:{lngBucket:F1}";
    }
}
