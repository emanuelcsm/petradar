namespace PetRadar.SharedKernel.Exceptions;

/// <summary>
/// The requested operation conflicts with the current state. Maps to HTTP 409.
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string errorCode, string message)
        : base(errorCode, message) { }
}
