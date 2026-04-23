using Microsoft.Extensions.FileProviders;

namespace PetRadar.API.Infrastructure;

internal static class MediaStaticFilesApplicationExtensions
{
    internal static IApplicationBuilder UseLocalMediaStaticFiles(this IApplicationBuilder app)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

        var localStorageBasePath = configuration["LocalStorage:BasePath"] ?? "./media-dev";
        var localStorageBaseUrl = configuration["LocalStorage:BaseUrl"] ?? "/media";
        var mediaPhysicalPath = Path.GetFullPath(localStorageBasePath);

        Directory.CreateDirectory(mediaPhysicalPath);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(mediaPhysicalPath),
            RequestPath = localStorageBaseUrl
        });

        return app;
    }
}
