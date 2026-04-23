using PetRadar.SharedKernel.Exceptions;

namespace Media.Domain.Exceptions;

public sealed class InvalidMediaMimeTypeException : DomainException
{
    public const string Code = "INVALID_MEDIA_MIME_TYPE";

    public InvalidMediaMimeTypeException()
        : base(Code, "Media mime type cannot be null or empty.")
    {
    }
}
