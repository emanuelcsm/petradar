namespace PetRadar.SharedKernel.Exceptions;

/// <summary>
/// The requested resource does not exist. Maps to HTTP 404.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string errorCode, string message)
        : base(errorCode, message) { }
}
