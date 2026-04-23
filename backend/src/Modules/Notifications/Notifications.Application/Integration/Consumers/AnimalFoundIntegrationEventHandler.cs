using MediatR;
using Notifications.Application.Interfaces;
using PetRadar.IntegrationEvents.Animals;

namespace Notifications.Application.Integration.Consumers;

public sealed class AnimalFoundIntegrationEventHandler : INotificationHandler<AnimalFoundIntegrationEvent>
{
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;

    public AnimalFoundIntegrationEventHandler(
        INotificationService notificationService,
        INotificationRepository notificationRepository)
    {
        _notificationService = notificationService;
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(AnimalFoundIntegrationEvent notification, CancellationToken cancellationToken)
    {
        const string eventName = "animal-found";

        var payload = new
        {
            notification.AnimalPostId,
            notification.UserId,
            notification.FoundAt
        };

        await _notificationService.SendToUserAsync(
            notification.UserId,
            eventName,
            payload,
            cancellationToken);

        var notificationRecord = new NotificationWriteModel(
            EventName: eventName,
            Message: "Seu animal foi marcado como encontrado.",
            CreatedAt: notification.FoundAt,
            UserId: notification.UserId,
            Payload: payload);

        await _notificationRepository.SaveAsync(notificationRecord, cancellationToken);
    }
}