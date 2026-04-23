using Animals.Application.Integration.Interfaces;
using Animals.Infrastructure.Persistence.Documents;
using MongoDB.Driver;

namespace Animals.Infrastructure.Persistence;

internal sealed class MongoKnownMediaRepository : IAnimalPostMediaRepository
{
    private readonly IMongoCollection<KnownMediaDocument> _collection;

    public MongoKnownMediaRepository(IMongoCollection<KnownMediaDocument> collection)
    {
        _collection = collection;
    }

    public async Task SaveAsync(
        string mediaId,
        string storagePath,
        CancellationToken cancellationToken = default)
    {
        var document = new KnownMediaDocument
        {
            Id = mediaId,
            StoragePath = storagePath
        };

        await _collection.ReplaceOneAsync(
            x => x.Id == document.Id,
            document,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(
        string mediaId,
        CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(
            x => x.Id == mediaId,
            cancellationToken: cancellationToken);

        return count > 0;
    }
}
