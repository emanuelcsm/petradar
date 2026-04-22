namespace PetRadar.SharedKernel.Events;

/// <summary>
/// Base for all domain events. Represents something that happened in the domain.
/// Payload must contain only IDs and scalar data — never domain objects or entities.
/// </summary>
public abstract record DomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
