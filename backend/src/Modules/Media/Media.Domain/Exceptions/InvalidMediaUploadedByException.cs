using PetRadar.SharedKernel.Exceptions;

namespace Media.Domain.Exceptions;

public sealed class InvalidMediaUploadedByException : DomainException
{
    public const string Code = "INVALID_MEDIA_UPLOADED_BY";

    public InvalidMediaUploadedByException()
        : base(Code, "UploadedBy cannot be null or empty.")
    {
    }
}
