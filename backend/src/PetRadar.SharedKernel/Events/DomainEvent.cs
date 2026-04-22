using MediatR;

namespace PetRadar.SharedKernel.Events;

/// <summary>
/// Base for all domain events. Represents something that happened in the domain.
/// Payload must contain only IDs and scalar data — never domain objects or entities.
/// Implements INotification so MediatR can dispatch events to INotificationHandler subscribers.
/// </summary>
public abstract record DomainEvent : INotification
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
