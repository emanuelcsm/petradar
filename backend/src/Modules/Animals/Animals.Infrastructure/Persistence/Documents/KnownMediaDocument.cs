using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Animals.Infrastructure.Persistence.Documents;

[BsonIgnoreExtraElements]
internal sealed class KnownMediaDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("storagePath")]
    public string StoragePath { get; set; } = default!;
}
