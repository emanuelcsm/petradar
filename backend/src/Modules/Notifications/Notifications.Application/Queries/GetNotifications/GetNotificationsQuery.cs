using MediatR;
using PetRadar.SharedKernel.Pagination;

namespace Notifications.Application.Queries.GetNotifications;

public sealed record GetNotificationsQuery(
    string UserId,
    int PageSize,
    int MaxPageSize,
    string? NextPageToken) : IRequest<CursorPage<GetNotificationsResult>>;
