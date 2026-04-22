using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class AnimalAlreadyFoundException : ConflictException
{
    public const string Code = "ANIMAL_ALREADY_FOUND";

    public AnimalAlreadyFoundException(string animalPostId)
        : base(Code, $"Animal post '{animalPostId}' is already marked as found.")
    {
    }
}