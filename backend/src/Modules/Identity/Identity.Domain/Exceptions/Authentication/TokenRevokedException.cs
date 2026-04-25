using PetRadar.SharedKernel.Exceptions;

namespace Identity.Domain.Exceptions.Authentication;

public sealed class TokenRevokedException : UnauthorizedException
{
    public const string Code = "TOKEN_REVOKED";

    public TokenRevokedException()
        : base(Code, "The provided token has been revoked.")
    {
    }
}
