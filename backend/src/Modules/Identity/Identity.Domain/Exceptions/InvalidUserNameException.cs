using PetRadar.SharedKernel.Exceptions;

namespace Identity.Domain.Exceptions;

/// <summary>
/// Raised when a user name fails validation.
/// Maps to HTTP 422.
/// </summary>
public sealed class InvalidUserNameException : DomainException
{
    public const string Code = "INVALID_USER_NAME";

    public InvalidUserNameException(string message)
        : base(Code, message) { }
}
