using MongoDB.Driver;

namespace PetRadar.API.Infrastructure;

internal static class MongoDbServiceExtensions
{
    internal static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(configuration["MongoDB:ConnectionString"]));

        services.AddSingleton<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>()
              .GetDatabase(configuration["MongoDB:DatabaseName"]));

        return services;
    }
}
