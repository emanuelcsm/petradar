using MediatR;
using Notifications.Application.Interfaces;
using PetRadar.IntegrationEvents.Animals;

namespace Notifications.Application.Integration.Consumers;

public sealed class AnimalFoundIntegrationEventHandler : INotificationHandler<AnimalFoundIntegrationEvent>
{
    private readonly INotificationService _notificationService;

    public AnimalFoundIntegrationEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(AnimalFoundIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var regionKey = BuildRegionKey(notification.Latitude, notification.Longitude);

        var payload = new
        {
            notification.AnimalPostId,
            Status = "Found",
            notification.FoundAt
        };

        await _notificationService.BroadcastToRegionAsync(
            regionKey,
            "animal-found",
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