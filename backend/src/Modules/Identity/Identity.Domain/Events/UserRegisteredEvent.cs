using PetRadar.SharedKernel.Events;

namespace Identity.Domain.Events;

/// <summary>
/// Published after a new user is successfully persisted.
/// Payload contains only IDs and scalar data — no domain objects.
/// </summary>
public sealed record UserRegisteredEvent : DomainEvent
{
    public string UserId { get; init; }
    public string Email { get; init; }
    public string Name { get; init; }

    public UserRegisteredEvent(string userId, string email, string name)
    {
        UserId = userId;
        Email = email;
        Name = name;
    }
}
