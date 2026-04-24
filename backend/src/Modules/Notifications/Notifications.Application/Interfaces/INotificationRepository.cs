using PetRadar.SharedKernel.Pagination;

namespace Notifications.Application.Interfaces;

public interface INotificationRepository
{
    Task SaveAsync(
        NotificationWriteModel notification,
        CancellationToken cancellationToken = default);

    Task<NotificationReadModel?> GetByIdForUserAsync(
        string notificationId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<CursorSlice<NotificationReadModel>> GetByUserIdAsync(
        string userId,
        DateTime? createdBeforeUtc,
        string? idBefore,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task MarkAsReadAsync(
        string notificationId,
        string userId,
        CancellationToken cancellationToken = default);
}