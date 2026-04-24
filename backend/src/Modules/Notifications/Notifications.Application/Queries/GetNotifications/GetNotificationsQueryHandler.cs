using MediatR;
using Notifications.Application.Exceptions;
using Notifications.Application.Interfaces;
using PetRadar.SharedKernel.Pagination;

namespace Notifications.Application.Queries.GetNotifications;

public sealed class GetNotificationsQueryHandler
    : IRequestHandler<GetNotificationsQuery, CursorPage<GetNotificationsResult>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<CursorPage<GetNotificationsResult>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PageSize <= 0 || request.PageSize > request.MaxPageSize)
            throw new InvalidNotificationsPageSizeException(request.PageSize, request.MaxPageSize);

        var cursor = NotificationsPageTokenCodec.Decode(request.NextPageToken);

        var notificationsSlice = await _notificationRepository.GetByUserIdAsync(
            userId: request.UserId,
            createdBeforeUtc: cursor?.CreatedAtUtc,
            idBefore: cursor?.Id,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var pageItems = notificationsSlice.Items;
        var hasNextPage = notificationsSlice.HasNextPage;

        var nextPageToken = hasNextPage
            ? NotificationsPageTokenCodec.Encode(pageItems[^1].CreatedAt, pageItems[^1].Id)
            : null;

        var data = pageItems
            .Select(notification => new GetNotificationsResult(
                Id: notification.Id,
                EventName: notification.EventName,
                Message: notification.Message,
                CreatedAt: notification.CreatedAt,
                Read: notification.Read,
                TipSenderName: notification.TipSenderName,
                TipMessage: notification.TipMessage))
            .ToList();

        return new CursorPage<GetNotificationsResult>(
            Data: data,
            NextPageToken: nextPageToken,
            HasNextPage: hasNextPage);
    }
}
