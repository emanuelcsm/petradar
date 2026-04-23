namespace Notifications.Application.Interfaces;

public interface INotificationService
{
    Task BroadcastToRegionAsync(
        string regionKey,
        string eventName,
        object payload,
        CancellationToken cancellationToken = default);

    Task SendToUserAsync(
        string userId,
        string eventName,
        object payload,
        CancellationToken cancellationToken = default);
}