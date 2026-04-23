using PetRadar.SharedKernel.Exceptions;

namespace Media.Domain.Exceptions;

public sealed class InvalidMediaStoragePathException : DomainException
{
    public const string Code = "INVALID_MEDIA_STORAGE_PATH";

    public InvalidMediaStoragePathException()
        : base(Code, "Media storage path cannot be null or empty.")
    {
    }
}
