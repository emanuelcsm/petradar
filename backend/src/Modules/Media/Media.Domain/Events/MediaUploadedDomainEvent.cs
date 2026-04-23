using PetRadar.SharedKernel.Events;

namespace Media.Domain.Events;

public sealed record MediaUploadedDomainEvent : DomainEvent
{
    public string MediaId { get; init; }
    public string StoragePath { get; init; }
    public string UploadedBy { get; init; }

    public MediaUploadedDomainEvent(
        string mediaId,
        string storagePath,
        string uploadedBy)
    {
        MediaId = mediaId;
        StoragePath = storagePath;
        UploadedBy = uploadedBy;
    }
}
