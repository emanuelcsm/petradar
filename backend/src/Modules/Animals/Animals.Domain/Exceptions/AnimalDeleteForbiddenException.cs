using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class AnimalDeleteForbiddenException : ForbiddenException
{
    public const string Code = "ANIMAL_DELETE_FORBIDDEN";

    public AnimalDeleteForbiddenException(string animalPostId, string requesterUserId)
        : base(Code, $"User '{requesterUserId}' is not allowed to delete animal post '{animalPostId}'.")
    {
    }
}
