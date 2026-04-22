using PetRadar.SharedKernel.Exceptions;

namespace Identity.Domain.Exceptions;

/// <summary>
/// Raised when a password hash value is invalid (null or empty).
/// Maps to HTTP 422.
/// </summary>
public sealed class InvalidPasswordHashException : DomainException
{
    public const string Code = "INVALID_PASSWORD_HASH";

    public InvalidPasswordHashException(string message)
        : base(Code, message) { }
}
