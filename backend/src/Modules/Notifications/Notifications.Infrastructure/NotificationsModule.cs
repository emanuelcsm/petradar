using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Notifications.Application.Integration.Consumers;
using Notifications.Application.Interfaces;
using Notifications.Infrastructure.Persistence;
using Notifications.Infrastructure.Persistence.Documents;
using Notifications.Infrastructure.Realtime;

namespace Notifications.Infrastructure;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<AnimalPostedIntegrationEventHandler>());

        services.AddSingleton(sp =>
        {
            var collection = sp.GetRequiredService<IMongoDatabase>()
                .GetCollection<NotificationDocument>("notifications_items");

            var userReadCreatedAtIndex = new CreateIndexModel<NotificationDocument>(
                Builders<NotificationDocument>.IndexKeys
                    .Ascending(x => x.UserId)
                    .Ascending(x => x.Read)
                    .Descending(x => x.CreatedAt),
                new CreateIndexOptions { Name = "ix_notifications_items_userId_read_createdAt" });

            collection.Indexes.CreateOne(userReadCreatedAtIndex);
            return collection;
        });

        services.AddScoped<INotificationService, SignalRNotificationService>();
        services.AddScoped<INotificationRepository, MongoNotificationRepository>();

        return services;
    }
}