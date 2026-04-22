namespace PetRadar.API.Infrastructure.Middleware;

internal static class GlobalExceptionMiddlewareExtensions
{
    internal static IApplicationBuilder UseGlobalExceptionMiddleware(
        this IApplicationBuilder app) =>
        app.UseMiddleware<GlobalExceptionMiddleware>();
}
