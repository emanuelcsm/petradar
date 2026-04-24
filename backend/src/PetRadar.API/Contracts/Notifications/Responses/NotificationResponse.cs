namespace PetRadar.API.Contracts.Notifications.Responses;

public sealed record NotificationResponse(
    string Id,
    string EventName,
    string Message,
    DateTime CreatedAt,
    bool Read);
