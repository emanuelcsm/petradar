using PetRadar.API.Infrastructure.Options;

namespace PetRadar.API.Infrastructure;

internal static class CorsServiceExtensions
{
    private const string FrontendCorsPolicyName = "FrontendCors";

    internal static IServiceCollection AddFrontendCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection(FrontendCorsOptions.SectionName)
            .Get<FrontendCorsOptions>()
            ?.AllowedOrigins
            ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendCorsPolicyName, policyBuilder =>
                policyBuilder
                    .WithOrigins(allowedOrigins.ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        return services;
    }

    internal static IApplicationBuilder UseFrontendCors(this IApplicationBuilder app)
    {
        app.UseCors(FrontendCorsPolicyName);

        return app;
    }
}
