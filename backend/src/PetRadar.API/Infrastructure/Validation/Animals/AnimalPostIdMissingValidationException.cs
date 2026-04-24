using PetRadar.SharedKernel.Exceptions;

namespace PetRadar.API.Infrastructure.Validation.Animals;

internal sealed class AnimalPostIdMissingValidationException : ValidationException
{
    public const string Code = "ANIMAL_POST_ID_MISSING";

    public AnimalPostIdMissingValidationException()
        : base(Code, "Animal post id is required.")
    {
    }
}
