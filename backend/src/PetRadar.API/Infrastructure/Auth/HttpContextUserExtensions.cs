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
}