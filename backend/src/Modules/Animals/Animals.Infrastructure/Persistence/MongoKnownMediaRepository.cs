using Animals.Application.Integration.Interfaces;
using Animals.Infrastructure.Persistence.Documents;
using MongoDB.Driver;

namespace Animals.Infrastructure.Persistence;

internal sealed class MongoKnownMediaRepository : IKnownMediaRepository
{
    private readonly IMongoCollection<KnownMediaDocument> _collection;

    public MongoKnownMediaRepository(IMongoCollection<KnownMediaDocument> collection)
    {
        _collection = collection;
    }

    public async Task SaveAsync(
        string mediaId,
        string publicUrl,
        string? storagePath = null,
        CancellationToken cancellationToken = default)
    {
        var document = new KnownMediaDocument
        {
            Id = mediaId,
            PublicUrl = publicUrl,
            StoragePath = storagePath
        };

        await _collection.ReplaceOneAsync(
            x => x.Id == document.Id,
            document,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task<IReadOnlyList<KnownMediaReadModel>> GetByIdsAsync(
        IReadOnlyCollection<string> mediaIds,
        CancellationToken cancellationToken = default)
    {
        if (mediaIds.Count == 0)
        {
            return [];
        }

        var documents = await _collection
            .Find(x => mediaIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return documents
            .Select(document => new KnownMediaReadModel(
                MediaId: document.Id,
                PublicUrl: document.PublicUrl))
            .ToList();
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
