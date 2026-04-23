namespace PetRadar.IntegrationEvents.Media;

public sealed record MediaUploadedIntegrationEvent : IntegrationEvent
{
    public string MediaId { get; init; }
    public string StoragePath { get; init; }
    public string UploadedBy { get; init; }

    public MediaUploadedIntegrationEvent(
        string mediaId,
        string storagePath,
        string uploadedBy)
    {
        MediaId = mediaId;
        StoragePath = storagePath;
        UploadedBy = uploadedBy;
    }
}
