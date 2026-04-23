using Media.Domain.Exceptions;
using PetRadar.SharedKernel.Entities;

namespace Media.Domain.Entities;

public sealed class MediaFile : AggregateRoot
{
    public string FileName { get; private set; } = default!;
    public string MimeType { get; private set; } = default!;
    public string StoragePath { get; private set; } = default!;
    public string UploadedBy { get; private set; } = default!;
    public DateTime CreatedAt { get; private init; }

    private MediaFile() { }

    private MediaFile(
        string id,
        string fileName,
        string mimeType,
        string storagePath,
        string uploadedBy,
        DateTime createdAt) : base(id)
    {
        FileName = fileName;
        MimeType = mimeType;
        StoragePath = storagePath;
        UploadedBy = uploadedBy;
        CreatedAt = createdAt;
    }

    public static MediaFile Create(
        string fileName,
        string mimeType,
        string storagePath,
        string uploadedBy)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidMediaFileNameException();

        if (string.IsNullOrWhiteSpace(mimeType))
            throw new InvalidMediaMimeTypeException();

        if (string.IsNullOrWhiteSpace(storagePath))
            throw new InvalidMediaStoragePathException();

        if (string.IsNullOrWhiteSpace(uploadedBy))
            throw new InvalidMediaUploadedByException();

        return new MediaFile(
            id: Guid.NewGuid().ToString(),
            fileName: fileName.Trim(),
            mimeType: mimeType.Trim(),
            storagePath: storagePath.Trim(),
            uploadedBy: uploadedBy.Trim(),
            createdAt: DateTime.UtcNow);
    }

    public static MediaFile Rehydrate(
        string id,
        string fileName,
        string mimeType,
        string storagePath,
        string uploadedBy,
        DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new InvalidMediaIdException();

        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidMediaFileNameException();

        if (string.IsNullOrWhiteSpace(mimeType))
            throw new InvalidMediaMimeTypeException();

        if (string.IsNullOrWhiteSpace(storagePath))
            throw new InvalidMediaStoragePathException();

        if (string.IsNullOrWhiteSpace(uploadedBy))
            throw new InvalidMediaUploadedByException();

        return new MediaFile(
            id: id.Trim(),
            fileName: fileName.Trim(),
            mimeType: mimeType.Trim(),
            storagePath: storagePath.Trim(),
            uploadedBy: uploadedBy.Trim(),
            createdAt: createdAt);
    }
}
