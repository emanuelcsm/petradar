using Microsoft.AspNetCore.SignalR;

namespace PetRadar.API.Hubs;

public sealed class AnimalHub : Hub
{
    public Task JoinRegion(string regionKey)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, regionKey);
    }

    public Task LeaveRegion(string regionKey)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, regionKey);
    }
}
