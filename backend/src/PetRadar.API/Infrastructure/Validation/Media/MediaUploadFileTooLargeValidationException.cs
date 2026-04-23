using PetRadar.SharedKernel.Exceptions;

namespace PetRadar.API.Infrastructure.Validation.Media;

internal sealed class MediaUploadFileTooLargeValidationException : ValidationException
{
    public const string Code = "MEDIA_UPLOAD_FILE_TOO_LARGE";

    public MediaUploadFileTooLargeValidationException(long fileSizeBytes, long maxAllowedSizeBytes)
        : base(Code, $"Uploaded media file size '{fileSizeBytes}' exceeds max allowed '{maxAllowedSizeBytes}' bytes.")
    {
    }
}
