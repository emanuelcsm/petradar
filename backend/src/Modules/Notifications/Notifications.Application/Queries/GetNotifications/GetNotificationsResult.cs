namespace Notifications.Application.Queries.GetNotifications;

public sealed record GetNotificationsResult(
    string Id,
    string EventName,
    string Message,
    DateTime CreatedAt,
    bool Read,
    string? TipSenderName = null,
    string? TipMessage = null);
