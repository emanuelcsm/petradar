using PetRadar.SharedKernel.Exceptions;

namespace PetRadar.API.Infrastructure.Validation.Animals;

internal sealed class TipMessageEmptyValidationException : ValidationException
{
    public const string Code = "TIP_MESSAGE_EMPTY";

    public TipMessageEmptyValidationException()
        : base(Code, "Tip message cannot be empty.") { }
}
