using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PetRadar.API.Infrastructure.Auth;

internal static class HttpContextUserExtensions
{
    internal static string GetRequiredUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(userId))
            throw new AuthenticatedUserIdMissingException();

        return userId;
    }

    internal static string GetRequiredUserName(this ClaimsPrincipal user)
    {
        var name = user.FindFirstValue(JwtRegisteredClaimNames.Name);

        if (string.IsNullOrWhiteSpace(name))
            throw new AuthenticatedUserNameMissingException();

        return name;
    }

    internal static string? GetJti(this ClaimsPrincipal user)
        => user.FindFirstValue(JwtRegisteredClaimNames.Jti);
}