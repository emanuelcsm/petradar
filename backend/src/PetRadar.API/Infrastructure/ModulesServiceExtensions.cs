using Animals.Infrastructure;
using Identity.Infrastructure;
using Media.Infrastructure;

namespace PetRadar.API.Infrastructure;

internal static class ModulesServiceExtensions
{
    internal static IServiceCollection AddApplicationModules(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAnimalsModule();
        services.AddIdentityModule(configuration);
        services.AddMediaModule(configuration);

        return services;
    }
}