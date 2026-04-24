namespace Notifications.Application.Interfaces;

public sealed record NotificationReadModel(
    string Id,
    string EventName,
    string Message,
    DateTime CreatedAt,
    bool Read,
    string? TipSenderName = null,
    string? TipMessage = null);
