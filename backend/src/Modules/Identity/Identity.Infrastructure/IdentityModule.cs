using Identity.Application.Commands.RegisterUser;
using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Auth;
using Identity.Application.Interfaces.Security;
using Identity.Infrastructure.Auth;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Documents;
using Identity.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Identity.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommandHandler>());

        services.AddSingleton(sp =>
        {
            var collection = sp.GetRequiredService<IMongoDatabase>()
                .GetCollection<UserDocument>("identity_users");

            var uniqueEmailIndex = new CreateIndexModel<UserDocument>(
                Builders<UserDocument>.IndexKeys.Ascending(x => x.Email),
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "ux_identity_users_email"
                });

            collection.Indexes.CreateOne(uniqueEmailIndex);
            return collection;
        });

        services.AddScoped<IUserRepository, MongoUserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
