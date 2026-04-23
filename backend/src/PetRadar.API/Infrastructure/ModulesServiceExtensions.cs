using Animals.Infrastructure;
using Identity.Infrastructure;
using Media.Infrastructure;
using Notifications.Infrastructure;

namespace PetRadar.API.Infrastructure;

internal static class ModulesServiceExtensions
{
    internal static IServiceCollection AddApplicationModules(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAnimalsModule();
        services.AddIdentityModule(configuration);
        services.AddNotificationsModule();
        services.AddMediaModule(configuration);

        return services;
    }
}