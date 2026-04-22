using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class InvalidAnimalPostIdException : DomainException
{
    public const string Code = "INVALID_ANIMAL_POST_ID";

    public InvalidAnimalPostIdException()
        : base(Code, "Animal post id cannot be null or empty.")
    {
    }
}