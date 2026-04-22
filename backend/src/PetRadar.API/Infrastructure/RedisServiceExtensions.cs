using StackExchange.Redis;

namespace PetRadar.API.Infrastructure;

internal static class RedisServiceExtensions
{
    internal static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]!));

        return services;
    }
}
