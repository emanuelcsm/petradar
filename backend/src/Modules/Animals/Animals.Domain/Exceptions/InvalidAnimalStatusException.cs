using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class InvalidAnimalStatusException : DomainException
{
    public const string Code = "INVALID_ANIMAL_STATUS";

    public InvalidAnimalStatusException(string value)
        : base(Code, $"'{value}' is not a valid animal status. Allowed values are 'Lost' and 'Found'.")
    {
    }
}