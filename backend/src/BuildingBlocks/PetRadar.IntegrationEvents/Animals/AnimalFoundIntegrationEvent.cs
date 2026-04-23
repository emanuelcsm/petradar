namespace PetRadar.IntegrationEvents.Animals;

public sealed record AnimalFoundIntegrationEvent : IntegrationEvent
{
    public string AnimalPostId { get; init; }
    public string UserId { get; init; }
    public DateTime FoundAt { get; init; }

    public AnimalFoundIntegrationEvent(
        string animalPostId,
        string userId,
        DateTime foundAt)
    {
        AnimalPostId = animalPostId;
        UserId = userId;
        FoundAt = foundAt;
    }
}
