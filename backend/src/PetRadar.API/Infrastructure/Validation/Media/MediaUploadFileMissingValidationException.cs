using PetRadar.SharedKernel.Exceptions;

namespace PetRadar.API.Infrastructure.Validation.Media;

internal sealed class MediaUploadFileMissingValidationException : ValidationException
{
    public const string Code = "MEDIA_UPLOAD_FILE_MISSING";

    public MediaUploadFileMissingValidationException()
        : base(Code, "A file is required for media upload.")
    {
    }
}
