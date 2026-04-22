namespace PetRadar.SharedKernel.Exceptions;

/// <summary>
/// The authenticated user is not allowed to perform the action. Maps to HTTP 403.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string errorCode, string message)
        : base(errorCode, message) { }
}
