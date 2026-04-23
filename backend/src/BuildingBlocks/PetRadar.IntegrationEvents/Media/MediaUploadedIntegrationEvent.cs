namespace PetRadar.IntegrationEvents.Media;

public sealed record MediaUploadedIntegrationEvent : IntegrationEvent
{
    public string MediaId { get; init; }
    public string PublicUrl { get; init; }
    public string StoragePath { get; init; }
    public string UploadedBy { get; init; }

    public MediaUploadedIntegrationEvent(
        string mediaId,
        string publicUrl,
        string storagePath,
        string uploadedBy)
    {
        MediaId = mediaId;
        PublicUrl = publicUrl;
        StoragePath = storagePath;
        UploadedBy = uploadedBy;
    }
}
