namespace PetRadar.SharedKernel.Exceptions;

/// <summary>
/// Base for all domain rule violations. Maps to HTTP 422 / 400.
/// </summary>
public abstract class DomainException : Exception
{
    public string ErrorCode { get; }

    protected DomainException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
