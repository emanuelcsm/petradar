using PetRadar.SharedKernel.Exceptions;

namespace PetRadar.API.Infrastructure.Validation.Animals;

internal sealed class TipMessageTooLongValidationException : ValidationException
{
    public const string Code = "TIP_MESSAGE_TOO_LONG";

    public TipMessageTooLongValidationException(int maxLength)
        : base(Code, $"Tip message cannot exceed {maxLength} characters.") { }
}
