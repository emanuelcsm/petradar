using Animals.Application.Interfaces;
using Animals.Domain.Entities;
using Animals.Infrastructure.Persistence.Documents;
using MongoDB.Driver;
using PetRadar.SharedKernel.Pagination;
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

    public async Task<CursorSlice<AnimalPost>> GetNearbyAsync(
        GeoLocation center,
        double radiusKm,
        DateTime? createdBeforeUtc,
        string? idBefore,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var geoFilter = Builders<AnimalPostDocument>.Filter.NearSphere(
            x => x.Location,
            center.Longitude,
            center.Latitude,
            maxDistance: radiusKm * 1000);

        var filter = geoFilter;

        if (createdBeforeUtc.HasValue && !string.IsNullOrWhiteSpace(idBefore))
        {
            var createdAtLessThan = Builders<AnimalPostDocument>.Filter.Lt(x => x.CreatedAt, createdBeforeUtc.Value);
            var tieBreaker = Builders<AnimalPostDocument>.Filter.And(
                Builders<AnimalPostDocument>.Filter.Eq(x => x.CreatedAt, createdBeforeUtc.Value),
                Builders<AnimalPostDocument>.Filter.Lt(x => x.Id, idBefore));

            var cursorFilter = Builders<AnimalPostDocument>.Filter.Or(createdAtLessThan, tieBreaker);
            filter = Builders<AnimalPostDocument>.Filter.And(geoFilter, cursorFilter);
        }

        var sort = Builders<AnimalPostDocument>.Sort
            .Descending(x => x.CreatedAt)
            .Descending(x => x.Id);

        var documents = await _collection
            .Find(filter)
            .Sort(sort)
            .Limit(pageSize + 1)
            .ToListAsync(cancellationToken);

        var items = documents
            .Select(document => document.ToDomain())
            .ToList();

        var hasNextPage = items.Count > pageSize;
        if (hasNextPage)
            items.RemoveAt(pageSize);

        return new CursorSlice<AnimalPost>(items, hasNextPage);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(
            x => x.Id == id,
            cancellationToken);
    }
}