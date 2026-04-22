using Animals.Application.Interfaces;
using Animals.Domain.Entities;
using Animals.Infrastructure.Persistence.Documents;
using MongoDB.Driver;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Infrastructure.Persistence;

internal sealed class MongoAnimalRepository : IAnimalRepository
{
    private readonly IMongoCollection<AnimalPostDocument> _collection;

    public MongoAnimalRepository(IMongoCollection<AnimalPostDocument> collection)
    {
        _collection = collection;
    }

    public async Task<AnimalPost?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var document = await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public async Task SaveAsync(AnimalPost animalPost, CancellationToken cancellationToken = default)
    {
        var document = AnimalPostDocument.FromDomain(animalPost);

        await _collection.ReplaceOneAsync(
            x => x.Id == document.Id,
            document,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task<IReadOnlyList<AnimalPost>> GetNearbyAsync(
        GeoLocation center,
        double radiusKm,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<AnimalPostDocument>.Filter.NearSphere(
            x => x.Location,
            center.Longitude,
            center.Latitude,
            maxDistance: radiusKm * 1000);

        var documents = await _collection
            .Find(filter)
            .ToListAsync(cancellationToken);

        return documents
            .Select(document => document.ToDomain())
            .ToList();
    }
}