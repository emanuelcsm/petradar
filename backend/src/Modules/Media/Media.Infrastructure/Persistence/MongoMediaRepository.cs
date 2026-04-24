using Media.Application.Interfaces.Persistence;
using Media.Domain.Entities;
using Media.Infrastructure.Persistence.Documents;
using MongoDB.Driver;

namespace Media.Infrastructure.Persistence;

internal sealed class MongoMediaRepository : IMediaRepository
{
    private readonly IMongoCollection<MediaDocument> _collection;

    public MongoMediaRepository(IMongoCollection<MediaDocument> collection)
    {
        _collection = collection;
    }

    public async Task SaveAsync(MediaFile mediaFile, CancellationToken cancellationToken = default)
    {
        var document = MediaDocument.FromDomain(mediaFile);

        await _collection.ReplaceOneAsync(
            x => x.Id == document.Id,
            document,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(string mediaId, CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(
            x => x.Id == mediaId,
            cancellationToken: cancellationToken);

        return count > 0;
    }

    public async Task<MediaFile?> GetByIdAsync(string mediaId, CancellationToken cancellationToken = default)
    {
        var document = await _collection
            .Find(x => x.Id == mediaId)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public async Task DeleteAsync(string mediaId, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(
            x => x.Id == mediaId,
            cancellationToken);
    }
}
