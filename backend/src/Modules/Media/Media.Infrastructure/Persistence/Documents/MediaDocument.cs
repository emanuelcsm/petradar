using Media.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Media.Infrastructure.Persistence.Documents;

[BsonIgnoreExtraElements]
internal sealed class MediaDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("fileName")]
    public string FileName { get; set; } = default!;

    [BsonElement("mimeType")]
    public string MimeType { get; set; } = default!;

    [BsonElement("storagePath")]
    public string StoragePath { get; set; } = default!;

    [BsonElement("uploadedBy")]
    public string UploadedBy { get; set; } = default!;

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    public static MediaDocument FromDomain(MediaFile mediaFile)
    {
        return new MediaDocument
        {
            Id = mediaFile.Id,
            FileName = mediaFile.FileName,
            MimeType = mediaFile.MimeType,
            StoragePath = mediaFile.StoragePath,
            UploadedBy = mediaFile.UploadedBy,
            CreatedAt = mediaFile.CreatedAt
        };
    }

    public MediaFile ToDomain()
    {
        return MediaFile.Rehydrate(
            id: Id,
            fileName: FileName,
            mimeType: MimeType,
            storagePath: StoragePath,
            uploadedBy: UploadedBy,
            createdAt: CreatedAt);
    }
}
