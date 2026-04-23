namespace Media.Application.Interfaces.Storage;

public interface IMediaStorage
{
    Task<string> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);

    string GetUrl(string storagePath);
}
