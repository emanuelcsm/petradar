using Identity.Application.Interfaces;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Documents;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Identity.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var collection = sp.GetRequiredService<IMongoDatabase>()
                .GetCollection<UserDocument>("identity_users");

            var uniqueEmailIndex = new CreateIndexModel<UserDocument>(
                Builders<UserDocument>.IndexKeys.Ascending(x => x.Email),
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "ux_identity_users_email"
                });

            collection.Indexes.CreateOne(uniqueEmailIndex);
            return collection;
        });

        services.AddScoped<IUserRepository, MongoUserRepository>();
        return services;
    }
}
