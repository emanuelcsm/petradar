namespace PetRadar.IntegrationEvents.Animals;

public sealed record AnimalFoundIntegrationEvent : IntegrationEvent
{
    public string AnimalPostId { get; init; }
    public string UserId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime FoundAt { get; init; }

    public AnimalFoundIntegrationEvent(
        string animalPostId,
        string userId,
        double latitude,
        double longitude,
        DateTime foundAt)
    {
        AnimalPostId = animalPostId;
        UserId = userId;
        Latitude = latitude;
        Longitude = longitude;
        FoundAt = foundAt;
    }
}
