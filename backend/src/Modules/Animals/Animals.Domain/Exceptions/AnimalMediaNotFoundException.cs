using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class AnimalMediaNotFoundException : NotFoundException
{
    public const string Code = "ANIMAL_MEDIA_NOT_FOUND";

    public AnimalMediaNotFoundException(string mediaId)
        : base(Code, $"Media '{mediaId}' is not known by Animals projection.")
    {
    }
}
