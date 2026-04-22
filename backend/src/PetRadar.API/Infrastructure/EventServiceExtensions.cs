using PetRadar.API.Infrastructure.Events;
using PetRadar.SharedKernel.Events;

namespace PetRadar.API.Infrastructure;

internal static class EventServiceExtensions
{
    internal static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        // Registers MediatR and scans this assembly for handlers.
        // Each AddXxxModule() will extend this registration with its own assembly.
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<Program>());

        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        return services;
    }
}
