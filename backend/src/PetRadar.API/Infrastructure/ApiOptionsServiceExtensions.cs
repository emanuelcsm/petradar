using PetRadar.API.Infrastructure.Options;

namespace PetRadar.API.Infrastructure;

internal static class ApiOptionsServiceExtensions
{
    internal static IServiceCollection AddApiOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<CursorPaginationOptions>()
            .Bind(configuration.GetSection(CursorPaginationOptions.SectionName))
            .Validate(options => options.DefaultPageSize > 0,
                "Pagination:Cursor:DefaultPageSize must be greater than zero.")
            .Validate(options => options.MaxPageSize >= options.DefaultPageSize,
                "Pagination:Cursor:MaxPageSize must be greater than or equal to DefaultPageSize.")
            .ValidateOnStart();

        return services;
    }
}