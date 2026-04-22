using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class InvalidAnimalUserIdException : DomainException
{
    public const string Code = "INVALID_ANIMAL_USER_ID";

    public InvalidAnimalUserIdException()
        : base(Code, "Animal post user id cannot be null or empty.")
    {
    }
}