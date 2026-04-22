namespace PetRadar.SharedKernel.Exceptions;

/// <summary>
/// The request requires authentication or the provided credentials are invalid. Maps to HTTP 401.
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string errorCode, string message)
        : base(errorCode, message) { }
}
