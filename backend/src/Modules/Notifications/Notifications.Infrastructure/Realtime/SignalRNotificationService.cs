using Microsoft.AspNetCore.SignalR;
using Notifications.Application.Interfaces;

namespace Notifications.Infrastructure.Realtime;

internal sealed class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<AnimalHub> _hubContext;

    public SignalRNotificationService(IHubContext<AnimalHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task BroadcastToRegionAsync(
        string regionKey,
        string eventName,
        object payload,
        CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group(regionKey)
            .SendAsync(eventName, payload, cancellationToken);
    }

    public Task SendToUserAsync(
        string userId,
        string eventName,
        object payload,
        CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.User(userId)
            .SendAsync(eventName, payload, cancellationToken);
    }
}