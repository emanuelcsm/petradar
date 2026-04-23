namespace PetRadar.API.Infrastructure.Options;

internal sealed class MediaUploadOptions
{
    internal const string SectionName = "MediaUpload";

    public long MaxFileSizeBytes { get; init; }
    public IReadOnlyList<string> AllowedMimeTypes { get; init; } = [];
}
