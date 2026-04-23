using PetRadar.API.Infrastructure.Options;
using PetRadar.API.Infrastructure.Validation.Media;

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

        services
            .AddOptions<MediaUploadOptions>()
            .Bind(configuration.GetSection(MediaUploadOptions.SectionName))
            .Validate(options => options.MaxFileSizeBytes > 0,
                "MediaUpload:MaxFileSizeBytes must be greater than zero.")
            .Validate(options => options.AllowedMimeTypes.Count > 0,
                "MediaUpload:AllowedMimeTypes must contain at least one mime type.")
            .Validate(options => options.AllowedMimeTypes.All(mime => !string.IsNullOrWhiteSpace(mime)),
                "MediaUpload:AllowedMimeTypes cannot contain empty values.")
            .ValidateOnStart();

        services
            .AddOptions<FrontendCorsOptions>()
            .Bind(configuration.GetSection(FrontendCorsOptions.SectionName))
            .Validate(options => options.AllowedOrigins.Count > 0,
                "Cors:Frontend:AllowedOrigins must contain at least one origin.")
            .Validate(options => options.AllowedOrigins.All(origin =>
                Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)),
                "Cors:Frontend:AllowedOrigins must contain only valid absolute HTTP/HTTPS origins.")
            .ValidateOnStart();

        services.AddScoped<IMediaUploadRequestValidator, MediaUploadRequestValidator>();

        return services;
    }
}