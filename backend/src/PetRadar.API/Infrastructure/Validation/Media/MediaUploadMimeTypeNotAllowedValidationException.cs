using PetRadar.SharedKernel.Exceptions;

namespace PetRadar.API.Infrastructure.Validation.Media;

internal sealed class MediaUploadMimeTypeNotAllowedValidationException : ValidationException
{
    public const string Code = "MEDIA_UPLOAD_MIME_TYPE_NOT_ALLOWED";

    public MediaUploadMimeTypeNotAllowedValidationException(string? receivedMimeType, IReadOnlyList<string> allowedMimeTypes)
        : base(Code, $"Uploaded media mime type '{receivedMimeType ?? "<empty>"}' is not allowed. Allowed: {string.Join(", ", allowedMimeTypes)}.")
    {
    }
}
