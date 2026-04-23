namespace Notifications.Application.Interfaces;

public interface INotificationRepository
{
    Task SaveAsync(
        NotificationWriteModel notification,
        CancellationToken cancellationToken = default);
}