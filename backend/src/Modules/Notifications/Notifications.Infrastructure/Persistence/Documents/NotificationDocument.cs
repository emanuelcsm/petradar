using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Notifications.Application.Interfaces;

namespace Notifications.Infrastructure.Persistence.Documents;

[BsonIgnoreExtraElements]
internal sealed class NotificationDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("eventName")]
    public string EventName { get; set; } = default!;

    [BsonElement("message")]
    public string Message { get; set; } = default!;

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonElement("read")]
    public bool Read { get; set; }

    [BsonElement("userId")]
    [BsonIgnoreIfNull]
    public string? UserId { get; set; }

    [BsonElement("regionKey")]
    [BsonIgnoreIfNull]
    public string? RegionKey { get; set; }

    [BsonElement("payload")]
    [BsonIgnoreIfNull]
    public BsonDocument? Payload { get; set; }

    [BsonElement("tipSenderName")]
    [BsonIgnoreIfNull]
    public string? TipSenderName { get; set; }

    [BsonElement("tipMessage")]
    [BsonIgnoreIfNull]
    public string? TipMessage { get; set; }

    public static NotificationDocument FromWriteModel(NotificationWriteModel model)
    {
        return new NotificationDocument
        {
            Id = ObjectId.GenerateNewId().ToString(),
            EventName = model.EventName,
            Message = model.Message,
            CreatedAt = model.CreatedAt,
            Read = false,
            UserId = model.UserId,
            RegionKey = model.RegionKey,
            Payload = model.Payload is null ? null : model.Payload.ToBsonDocument(),
            TipSenderName = model.TipSenderName,
            TipMessage = model.TipMessage,
        };
    }
}