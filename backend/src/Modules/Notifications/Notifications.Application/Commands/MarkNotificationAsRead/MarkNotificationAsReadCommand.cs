using MediatR;

namespace Notifications.Application.Commands.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(
    string NotificationId,
    string UserId) : IRequest;
