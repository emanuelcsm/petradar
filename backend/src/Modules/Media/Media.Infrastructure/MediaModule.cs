using Media.Application.Commands.UploadMedia;
using Media.Application.Interfaces.Persistence;
using Media.Application.Interfaces.Storage;
using Media.Infrastructure.Options;
using Media.Infrastructure.Persistence;
using Media.Infrastructure.Persistence.Documents;
using Media.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Media.Infrastructure;

public static class MediaModule
{
    public static IServiceCollection AddMediaModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<UploadMediaCommandHandler>());

        services.Configure<LocalStorageOptions>(
            configuration.GetSection(LocalStorageOptions.SectionName));

        services.AddSingleton(sp =>
            sp.GetRequiredService<IMongoDatabase>()
              .GetCollection<MediaDocument>("media_files"));

        services.AddScoped<IMediaStorage, LocalMediaStorage>();
        services.AddScoped<IMediaRepository, MongoMediaRepository>();

        return services;
    }
}
