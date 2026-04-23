using MediatR;

namespace PetRadar.IntegrationEvents;

public abstract record IntegrationEvent : INotification
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
