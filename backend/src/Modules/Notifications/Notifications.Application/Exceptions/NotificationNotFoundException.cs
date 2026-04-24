using PetRadar.SharedKernel.Exceptions;

namespace Notifications.Application.Exceptions;

public sealed class NotificationNotFoundException : NotFoundException
{
    public const string Code = "NOTIFICATION_NOT_FOUND";

    public NotificationNotFoundException(string notificationId)
        : base(Code, $"Notification '{notificationId}' was not found.")
    {
    }
}
