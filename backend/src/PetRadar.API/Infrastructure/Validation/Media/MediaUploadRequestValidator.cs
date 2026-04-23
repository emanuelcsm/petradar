using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PetRadar.API.Infrastructure.Options;

namespace PetRadar.API.Infrastructure.Validation.Media;

internal sealed class MediaUploadRequestValidator : IMediaUploadRequestValidator
{
    private readonly MediaUploadOptions _options;

    public MediaUploadRequestValidator(IOptions<MediaUploadOptions> options)
    {
        _options = options.Value;
    }

    public void Validate(IFormFile? file)
    {
        if (file is null)
            throw new MediaUploadFileMissingValidationException();

        if (file.Length <= 0)
            throw new MediaUploadFileEmptyValidationException();

        if (file.Length > _options.MaxFileSizeBytes)
            throw new MediaUploadFileTooLargeValidationException(file.Length, _options.MaxFileSizeBytes);

        if (string.IsNullOrWhiteSpace(file.ContentType))
            throw new MediaUploadMimeTypeNotAllowedValidationException(file.ContentType, _options.AllowedMimeTypes);

        var isAllowedMimeType = _options.AllowedMimeTypes
            .Any(mimeType => string.Equals(mimeType, file.ContentType, StringComparison.OrdinalIgnoreCase));

        if (!isAllowedMimeType)
            throw new MediaUploadMimeTypeNotAllowedValidationException(file.ContentType, _options.AllowedMimeTypes);
    }
}
