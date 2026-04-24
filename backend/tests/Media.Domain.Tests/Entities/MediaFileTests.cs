using Media.Domain.Entities;
using Media.Domain.Events;
using Media.Domain.Exceptions;

namespace Media.Domain.Tests.Entities;

public sealed class MediaFileTests
{
    private const string ValidFileName = "foto.jpg";
    private const string ValidMimeType = "image/jpeg";
    private const string ValidStoragePath = "uploads/foto.jpg";
    private const string ValidUploadedBy = "user-123";

    [Fact]
    public void Create_WithValidInputs_ReturnsMediaFileWithExpectedFields()
    {
        var file = MediaFile.Create(ValidFileName, ValidMimeType, ValidStoragePath, ValidUploadedBy);

        Assert.False(string.IsNullOrWhiteSpace(file.Id));
        Assert.Equal(ValidFileName, file.FileName);
        Assert.Equal(ValidMimeType, file.MimeType);
        Assert.Equal(ValidStoragePath, file.StoragePath);
        Assert.Equal(ValidUploadedBy, file.UploadedBy);
        Assert.True((DateTime.UtcNow - file.CreatedAt).TotalSeconds < 5);
    }

    [Fact]
    public void Create_WithNullFileName_ThrowsInvalidMediaFileNameException()
    {
        var act = () => MediaFile.Create(null!, ValidMimeType, ValidStoragePath, ValidUploadedBy);

        Assert.Throws<InvalidMediaFileNameException>(act);
    }

    [Fact]
    public void Create_WithNullMimeType_ThrowsInvalidMediaMimeTypeException()
    {
        var act = () => MediaFile.Create(ValidFileName, null!, ValidStoragePath, ValidUploadedBy);

        Assert.Throws<InvalidMediaMimeTypeException>(act);
    }

    [Fact]
    public void Create_WithNullStoragePath_ThrowsInvalidMediaStoragePathException()
    {
        var act = () => MediaFile.Create(ValidFileName, ValidMimeType, null!, ValidUploadedBy);

        Assert.Throws<InvalidMediaStoragePathException>(act);
    }

    [Fact]
    public void Create_WithNullUploadedBy_ThrowsInvalidMediaUploadedByException()
    {
        var act = () => MediaFile.Create(ValidFileName, ValidMimeType, ValidStoragePath, null!);

        Assert.Throws<InvalidMediaUploadedByException>(act);
    }

    [Fact]
    public void Create_AccumulatesExactlyOneMediaUploadedDomainEvent()
    {
        var file = MediaFile.Create(ValidFileName, ValidMimeType, ValidStoragePath, ValidUploadedBy);

        var events = file.CollectDomainEvents();

        var domainEvent = Assert.Single(events);
        Assert.IsType<MediaUploadedDomainEvent>(domainEvent);
    }

    [Fact]
    public void Create_MediaUploadedDomainEventContainsCorrectPayload()
    {
        var file = MediaFile.Create(ValidFileName, ValidMimeType, ValidStoragePath, ValidUploadedBy);

        var events = file.CollectDomainEvents();
        var uploadedEvent = Assert.IsType<MediaUploadedDomainEvent>(Assert.Single(events));

        Assert.Equal(file.Id, uploadedEvent.MediaId);
        Assert.Equal(ValidStoragePath, uploadedEvent.StoragePath);
        Assert.Equal(ValidUploadedBy, uploadedEvent.UploadedBy);
    }

    [Fact]
    public void Create_TwoMediaFiles_HaveDifferentIds()
    {
        var first = MediaFile.Create(ValidFileName, ValidMimeType, ValidStoragePath, ValidUploadedBy);
        var second = MediaFile.Create(ValidFileName, ValidMimeType, ValidStoragePath, ValidUploadedBy);

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void CollectDomainEvents_CalledTwice_ReturnsEmptyOnSecondCall()
    {
        var file = MediaFile.Create(ValidFileName, ValidMimeType, ValidStoragePath, ValidUploadedBy);
        file.CollectDomainEvents();

        var secondCollection = file.CollectDomainEvents();

        Assert.Empty(secondCollection);
    }
}
