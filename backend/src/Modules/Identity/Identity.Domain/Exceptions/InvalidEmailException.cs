using PetRadar.SharedKernel.Exceptions;

namespace Identity.Domain.Exceptions;

/// <summary>
/// Raised when an email address fails format validation.
/// Maps to HTTP 422.
/// </summary>
public sealed class InvalidEmailException : DomainException
{
    public const string Code = "INVALID_EMAIL";

    public InvalidEmailException(string message)
        : base(Code, message) { }
}
