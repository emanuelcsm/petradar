namespace PetRadar.IntegrationEvents.Identity;

public sealed record UserRegisteredIntegrationEvent : IntegrationEvent
{
    public string UserId { get; init; }
    public string Email { get; init; }
    public string Name { get; init; }
    public DateTime RegisteredAt { get; init; }

    public UserRegisteredIntegrationEvent(
        string userId,
        string email,
        string name,
        DateTime registeredAt)
    {
        UserId = userId;
        Email = email;
        Name = name;
        RegisteredAt = registeredAt;
    }
}
