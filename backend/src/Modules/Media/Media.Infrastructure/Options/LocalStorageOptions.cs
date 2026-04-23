namespace Media.Infrastructure.Options;

internal sealed class LocalStorageOptions
{
    public const string SectionName = "LocalStorage";

    public string BasePath { get; init; } = default!;
    public string BaseUrl { get; init; } = default!;
}
