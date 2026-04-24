namespace PetRadar.API.Infrastructure.Validation.Animals;

internal sealed class SendTipValidator : ISendTipValidator
{
    private const int MaxLength = 280;

    public void Validate(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new TipMessageEmptyValidationException();

        if (message.Length > MaxLength)
            throw new TipMessageTooLongValidationException(MaxLength);
    }
}
