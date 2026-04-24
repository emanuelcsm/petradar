namespace Notifications.Application.Interfaces;

public sealed record NotificationWriteModel(
    string EventName,
    string Message,
    DateTime CreatedAt,
    string? UserId = null,
    string? RegionKey = null,
    object? Payload = null,
    string? TipSenderName = null,
    string? TipMessage = null);