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

/// <summary>
/// The requested resource does not exist. Maps to HTTP 404.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string errorCode, string message)
        : base(errorCode, message) { }
}

/// <summary>
/// The requested operation conflicts with the current state. Maps to HTTP 409.
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string errorCode, string message)
        : base(errorCode, message) { }
}

/// <summary>
/// The authenticated user is not allowed to perform the action. Maps to HTTP 403.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string errorCode, string message)
        : base(errorCode, message) { }
}
