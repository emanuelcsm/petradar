namespace PetRadar.SharedKernel.Exceptions;

/// <summary>
/// The request payload is syntactically or structurally invalid. Maps to HTTP 400.
/// </summary>
public abstract class ValidationException : DomainException
{
    protected ValidationException(string errorCode, string message)
        : base(errorCode, message)
    {
    }
}
