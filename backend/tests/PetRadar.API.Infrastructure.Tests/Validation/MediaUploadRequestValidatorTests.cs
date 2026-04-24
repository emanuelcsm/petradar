using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSubstitute;
using PetRadar.API.Infrastructure.Options;
using PetRadar.API.Infrastructure.Validation.Media;

namespace PetRadar.API.Infrastructure.Tests.Validation;

public sealed class MediaUploadRequestValidatorTests
{
    private static readonly MediaUploadOptions DefaultOptions = new()
    {
        MaxFileSizeBytes = 5_000_000,
        AllowedMimeTypes = ["image/jpeg", "image/png"]
    };

    private static MediaUploadRequestValidator CreateValidator(MediaUploadOptions? options = null) =>
        new(Microsoft.Extensions.Options.Options.Create(options ?? DefaultOptions));

    private static IFormFile CreateFile(long length, string contentType)
    {
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(length);
        file.ContentType.Returns(contentType);
        return file;
    }

    [Fact]
    public void Validate_WithNullFile_ThrowsMediaUploadFileMissingValidationException()
    {
        var act = () => CreateValidator().Validate(null);

        Assert.Throws<MediaUploadFileMissingValidationException>(act);
    }

    [Fact]
    public void Validate_WithEmptyFile_ThrowsMediaUploadFileEmptyValidationException()
    {
        var file = CreateFile(length: 0, contentType: "image/jpeg");

        var act = () => CreateValidator().Validate(file);

        Assert.Throws<MediaUploadFileEmptyValidationException>(act);
    }

    [Fact]
    public void Validate_WithOversizedFile_ThrowsMediaUploadFileTooLargeValidationException()
    {
        var file = CreateFile(length: 10_000_000, contentType: "image/jpeg");

        var act = () => CreateValidator().Validate(file);

        Assert.Throws<MediaUploadFileTooLargeValidationException>(act);
    }

    [Fact]
    public void Validate_WithDisallowedMimeType_ThrowsMediaUploadMimeTypeNotAllowedValidationException()
    {
        var file = CreateFile(length: 1_000, contentType: "application/pdf");

        var act = () => CreateValidator().Validate(file);

        Assert.Throws<MediaUploadMimeTypeNotAllowedValidationException>(act);
    }

    [Fact]
    public void Validate_WithValidFile_DoesNotThrow()
    {
        var file = CreateFile(length: 1_000, contentType: "image/jpeg");

        var ex = Record.Exception(() => CreateValidator().Validate(file));

        Assert.Null(ex);
    }
}
