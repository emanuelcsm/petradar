using MediatR;
using Notifications.Application.Exceptions;
using Notifications.Application.Interfaces;

namespace Notifications.Application.Commands.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand>
{
    private readonly INotificationRepository _notificationRepository;

    public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdForUserAsync(
            notificationId: request.NotificationId,
            userId: request.UserId,
            cancellationToken: cancellationToken);

        if (notification is null)
            throw new NotificationNotFoundException(request.NotificationId);

        await _notificationRepository.MarkAsReadAsync(
            notificationId: request.NotificationId,
            userId: request.UserId,
            cancellationToken: cancellationToken);
    }
}
