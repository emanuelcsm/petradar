using Notifications.Infrastructure.Realtime;

namespace PetRadar.API.Infrastructure;

internal static class RealtimeEndpointExtensions
{
    internal static IEndpointRouteBuilder MapRealtimeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<AnimalHub>("/hubs/animals");

        return endpoints;
    }
}
