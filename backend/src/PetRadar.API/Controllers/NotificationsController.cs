using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Notifications.Application.Commands.MarkNotificationAsRead;
using Notifications.Application.Queries.GetNotifications;
using PetRadar.API.Contracts.Notifications.Responses;
using PetRadar.API.Infrastructure.Auth;
using PetRadar.API.Infrastructure.Options;
using PetRadar.API.Infrastructure.Responses;

namespace PetRadar.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly CursorPaginationOptions _cursorPaginationOptions;

    public NotificationsController(
        ISender sender,
        IOptions<CursorPaginationOptions> cursorPaginationOptions)
    {
        _sender = sender;
        _cursorPaginationOptions = cursorPaginationOptions.Value;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] string? nextPageToken,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var effectivePageSize = pageSize ?? _cursorPaginationOptions.DefaultPageSize;

        var query = new GetNotificationsQuery(
            UserId: userId,
            PageSize: effectivePageSize,
            MaxPageSize: _cursorPaginationOptions.MaxPageSize,
            NextPageToken: nextPageToken);

        var result = await _sender.Send(query, cancellationToken);

        var response = result.Data
            .Select(notification => new NotificationResponse(
                Id: notification.Id,
                EventName: notification.EventName,
                Message: notification.Message,
                CreatedAt: notification.CreatedAt,
                Read: notification.Read,
                TipSenderName: notification.TipSenderName,
                TipMessage: notification.TipMessage))
            .ToList();

        return Ok(response.ToPagedResponse(
            nextPageToken: result.NextPageToken,
            hasNextPage: result.HasNextPage));
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();

        var command = new MarkNotificationAsReadCommand(
            NotificationId: id,
            UserId: userId);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
}
