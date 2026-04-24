using Microsoft.AspNetCore.SignalR;
using PetRadar.API.Infrastructure.Auth;

namespace PetRadar.API.Infrastructure;

internal static class RealtimeServiceExtensions
{
    internal static IServiceCollection AddRealtime(this IServiceCollection services)
    {
        services.AddSingleton<IUserIdProvider, SubClaimUserIdProvider>();
        services.AddSignalR();

        return services;
    }
}
