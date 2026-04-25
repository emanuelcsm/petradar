using Identity.Application.Interfaces.Auth;
using Identity.Domain.Exceptions.Authentication;
using PetRadar.API.Infrastructure.Auth;

namespace PetRadar.API.Infrastructure.Middleware;

internal sealed class JwtRevocationMiddleware
{
    private readonly RequestDelegate _next;

    public JwtRevocationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenBlacklist tokenBlacklist)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var jti = context.User.GetJti();

            if (jti is null || await tokenBlacklist.IsRevokedAsync(jti, context.RequestAborted))
                throw new TokenRevokedException();
        }

        await _next(context);
    }
}
