using PetRadar.SharedKernel.Exceptions;

namespace PetRadar.API.Infrastructure.Auth;

internal sealed class AuthenticatedUserIdMissingException : UnauthorizedException
{
    public const string Code = "AUTHENTICATED_USER_ID_MISSING";

    public AuthenticatedUserIdMissingException()
        : base(Code, "Authenticated user id claim is missing or invalid.")
    {
    }
}