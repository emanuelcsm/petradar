using Media.Application.Interfaces.Storage;
using Media.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Media.Infrastructure.Storage;

internal sealed class LocalMediaStorage : IMediaStorage
{
    private readonly LocalStorageOptions _options;

    public LocalMediaStorage(IOptions<LocalStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid()}{extension}";

        var datePath = DateTime.UtcNow.ToString("yyyy/MM");
        var relativePath = Path.Combine(datePath, uniqueName).Replace('\\', '/');

        var fullDirectory = Path.Combine(_options.BasePath, datePath);
        Directory.CreateDirectory(fullDirectory);

        var fullPath = Path.Combine(_options.BasePath, datePath, uniqueName);

        await using var fileStream = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await content.CopyToAsync(fileStream, cancellationToken);

        return relativePath;
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_options.BasePath, storagePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public string GetUrl(string storagePath)
    {
        return $"{_options.BaseUrl.TrimEnd('/')}/{storagePath.TrimStart('/')}";
    }
}
