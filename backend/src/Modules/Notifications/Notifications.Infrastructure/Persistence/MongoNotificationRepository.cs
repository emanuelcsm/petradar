using MongoDB.Driver;
using Notifications.Application.Interfaces;
using Notifications.Infrastructure.Persistence.Documents;
using PetRadar.SharedKernel.Pagination;

namespace Notifications.Infrastructure.Persistence;

internal sealed class MongoNotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<NotificationDocument> _collection;

    public MongoNotificationRepository(IMongoCollection<NotificationDocument> collection)
    {
        _collection = collection;
    }

    public Task SaveAsync(
        NotificationWriteModel notification,
        CancellationToken cancellationToken = default)
    {
        var document = NotificationDocument.FromWriteModel(notification);
        return _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
    }

    public async Task<NotificationReadModel?> GetByIdForUserAsync(
        string notificationId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var document = await _collection
            .Find(x => x.Id == notificationId && x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        return document is null ? null : ToReadModel(document);
    }

    public async Task<CursorSlice<NotificationReadModel>> GetByUserIdAsync(
        string userId,
        DateTime? createdBeforeUtc,
        string? idBefore,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var userFilter = Builders<NotificationDocument>.Filter.Eq(x => x.UserId, userId);
        var filter = userFilter;

        if (createdBeforeUtc.HasValue && !string.IsNullOrWhiteSpace(idBefore))
        {
            var createdAtLessThan = Builders<NotificationDocument>.Filter.Lt(x => x.CreatedAt, createdBeforeUtc.Value);
            var tieBreaker = Builders<NotificationDocument>.Filter.And(
                Builders<NotificationDocument>.Filter.Eq(x => x.CreatedAt, createdBeforeUtc.Value),
                Builders<NotificationDocument>.Filter.Lt(x => x.Id, idBefore));

            var cursorFilter = Builders<NotificationDocument>.Filter.Or(createdAtLessThan, tieBreaker);
            filter = Builders<NotificationDocument>.Filter.And(userFilter, cursorFilter);
        }

        var sort = Builders<NotificationDocument>.Sort
            .Descending(x => x.CreatedAt)
            .Descending(x => x.Id);

        var documents = await _collection
            .Find(filter)
            .Sort(sort)
            .Limit(pageSize + 1)
            .ToListAsync(cancellationToken);

        var items = documents
            .Select(ToReadModel)
            .ToList();

        var hasNextPage = items.Count > pageSize;
        if (hasNextPage)
            items.RemoveAt(pageSize);

        return new CursorSlice<NotificationReadModel>(items, hasNextPage);
    }

    public Task MarkAsReadAsync(
        string notificationId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<NotificationDocument>.Filter.And(
            Builders<NotificationDocument>.Filter.Eq(x => x.Id, notificationId),
            Builders<NotificationDocument>.Filter.Eq(x => x.UserId, userId));

        var update = Builders<NotificationDocument>.Update
            .Set(x => x.Read, true);

        return _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    private static NotificationReadModel ToReadModel(NotificationDocument document)
    {
        return new NotificationReadModel(
            Id: document.Id,
            EventName: document.EventName,
            Message: document.Message,
            CreatedAt: document.CreatedAt,
            Read: document.Read,
            Payload: document.Payload);
    }
}