using PetRadar.SharedKernel.Exceptions;

namespace Media.Domain.Exceptions;

public sealed class InvalidMediaFileNameException : DomainException
{
    public const string Code = "INVALID_MEDIA_FILE_NAME";

    public InvalidMediaFileNameException()
        : base(Code, "Media file name cannot be null or empty.")
    {
    }
}
