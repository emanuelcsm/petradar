namespace Notifications.Application.Interfaces;

public sealed record NotificationReadModel(
    string Id,
    string EventName,
    string Message,
    DateTime CreatedAt,
    bool Read,
    object? Payload = null);
