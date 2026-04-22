using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class InvalidAnimalDescriptionException : DomainException
{
    public const string Code = "INVALID_ANIMAL_DESCRIPTION";

    public InvalidAnimalDescriptionException(string message)
        : base(Code, message)
    {
    }
}