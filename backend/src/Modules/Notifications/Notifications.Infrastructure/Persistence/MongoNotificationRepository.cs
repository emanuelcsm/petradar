using MongoDB.Driver;
using Notifications.Application.Interfaces;
using Notifications.Infrastructure.Persistence.Documents;

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
}