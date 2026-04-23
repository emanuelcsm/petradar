using PetRadar.SharedKernel.Exceptions;

namespace Media.Domain.Exceptions;

public sealed class InvalidMediaIdException : DomainException
{
    public const string Code = "INVALID_MEDIA_ID";

    public InvalidMediaIdException()
        : base(Code, "Media id cannot be null or empty.")
    {
    }
}
