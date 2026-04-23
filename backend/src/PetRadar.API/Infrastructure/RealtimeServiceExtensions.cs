namespace PetRadar.API.Infrastructure;

internal static class RealtimeServiceExtensions
{
    internal static IServiceCollection AddRealtime(this IServiceCollection services)
    {
        services.AddSignalR();

        return services;
    }
}
