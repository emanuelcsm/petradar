using PetRadar.SharedKernel.Exceptions;

namespace PetRadar.API.Infrastructure.Validation.Media;

internal sealed class MediaUploadFileEmptyValidationException : ValidationException
{
    public const string Code = "MEDIA_UPLOAD_FILE_EMPTY";

    public MediaUploadFileEmptyValidationException()
        : base(Code, "Uploaded media file cannot be empty.")
    {
    }
}
