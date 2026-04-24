using MediatR;
using Notifications.Application.Interfaces;
using PetRadar.IntegrationEvents.Animals;

namespace Notifications.Application.Integration.Consumers;

public sealed class AnimalTipSentIntegrationEventHandler : INotificationHandler<AnimalTipSentIntegrationEvent>
{
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;

    public AnimalTipSentIntegrationEventHandler(
        INotificationService notificationService,
        INotificationRepository notificationRepository)
    {
        _notificationService    = notificationService;
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(AnimalTipSentIntegrationEvent notification, CancellationToken cancellationToken)
    {
        const string eventName = "new-notification";

        var payload = new
        {
            notification.AnimalPostId,
            notification.SenderName,
            notification.Message
        };

        var notificationRecord = new NotificationWriteModel(
            EventName: eventName,
            Message:   $"{notification.SenderName} enviou uma dica sobre o seu animal.",
            CreatedAt: notification.OccurredOn,
            UserId:    notification.OwnerId,
            Payload:   payload);

        await _notificationRepository.SaveAsync(notificationRecord, cancellationToken);

        await _notificationService.SendToUserAsync(
            notification.OwnerId,
            eventName,
            payload,
            cancellationToken);
    }
}
