using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class AnimalPostNotFoundException : NotFoundException
{
    public const string Code = "ANIMAL_POST_NOT_FOUND";

    public AnimalPostNotFoundException(string animalPostId)
        : base(Code, $"Animal post '{animalPostId}' was not found.")
    {
    }
}
