using MediatR;
using Notifications.Application.Interfaces;
using PetRadar.IntegrationEvents.Animals;

namespace Notifications.Application.Integration.Consumers;

public sealed class AnimalPostDeletedIntegrationEventHandler : INotificationHandler<AnimalPostDeletedIntegrationEvent>
{
    private readonly INotificationService _notificationService;

    public AnimalPostDeletedIntegrationEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(AnimalPostDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var regionKey = BuildRegionKey(notification.Latitude, notification.Longitude);

        var payload = new
        {
            notification.AnimalPostId
        };

        await _notificationService.BroadcastToRegionAsync(
            regionKey,
            "animal-deleted",
            payload,
            cancellationToken);
    }

    private static string BuildRegionKey(double latitude, double longitude)
    {
        var latBucket = Math.Round(latitude, 1, MidpointRounding.AwayFromZero);
        var lngBucket = Math.Round(longitude, 1, MidpointRounding.AwayFromZero);

        return $"region:{latBucket:F1}:{lngBucket:F1}";
    }
}
