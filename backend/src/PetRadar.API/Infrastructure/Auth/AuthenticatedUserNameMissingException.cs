using PetRadar.SharedKernel.Exceptions;

namespace PetRadar.API.Infrastructure.Auth;

internal sealed class AuthenticatedUserNameMissingException : UnauthorizedException
{
    public const string Code = "AUTHENTICATED_USER_NAME_MISSING";

    public AuthenticatedUserNameMissingException()
        : base(Code, "Authenticated user name claim is missing or invalid.") { }
}
