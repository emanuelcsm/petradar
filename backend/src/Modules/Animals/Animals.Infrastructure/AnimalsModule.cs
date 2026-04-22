using Animals.Application.Commands.CreateAnimalPost;
using Animals.Application.Interfaces;
using Animals.Infrastructure.Persistence;
using Animals.Infrastructure.Persistence.Documents;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Animals.Infrastructure;

public static class AnimalsModule
{
    public static IServiceCollection AddAnimalsModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<CreateAnimalPostCommandHandler>());

        services.AddSingleton(sp =>
        {
            var collection = sp.GetRequiredService<IMongoDatabase>()
                .GetCollection<AnimalPostDocument>("animals_posts");

            var geoIndex = new CreateIndexModel<AnimalPostDocument>(
                Builders<AnimalPostDocument>.IndexKeys.Geo2DSphere(x => x.Location),
                new CreateIndexOptions { Name = "ix_animals_posts_location_2dsphere" });

            var statusCreatedAtIndex = new CreateIndexModel<AnimalPostDocument>(
                Builders<AnimalPostDocument>.IndexKeys
                    .Ascending(x => x.Status)
                    .Descending(x => x.CreatedAt),
                new CreateIndexOptions { Name = "ix_animals_posts_status_createdAt" });

            collection.Indexes.CreateOne(geoIndex);
            collection.Indexes.CreateOne(statusCreatedAtIndex);

            return collection;
        });

        services.AddScoped<IAnimalRepository, MongoAnimalRepository>();

        return services;
    }
}