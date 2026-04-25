namespace PetRadar.API.Infrastructure.Middleware;

internal static class JwtRevocationMiddlewareExtensions
{
    internal static IApplicationBuilder UseJwtRevocation(this IApplicationBuilder app)
        => app.UseMiddleware<JwtRevocationMiddleware>();
}
