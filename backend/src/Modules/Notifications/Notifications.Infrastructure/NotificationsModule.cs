using Microsoft.Extensions.DependencyInjection;
using Notifications.Application.Integration.Consumers;

namespace Notifications.Infrastructure;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<AnimalPostedIntegrationEventHandler>());

        return services;
    }
}