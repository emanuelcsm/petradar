using PetRadar.SharedKernel.Exceptions;

namespace Identity.Domain.Exceptions.Authentication;

public sealed class InvalidCredentialsException : UnauthorizedException
{
    public const string Code = "INVALID_CREDENTIALS";

    public InvalidCredentialsException()
        : base(Code, "Email or password is incorrect.")
    {
    }
}
