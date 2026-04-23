namespace PetRadar.IntegrationEvents.Animals;

public sealed record AnimalPostedIntegrationEvent : IntegrationEvent
{
    public string AnimalPostId { get; init; }
    public string UserId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime CreatedAt { get; init; }

    public AnimalPostedIntegrationEvent(
        string animalPostId,
        string userId,
        double latitude,
        double longitude,
        DateTime createdAt)
    {
        AnimalPostId = animalPostId;
        UserId = userId;
        Latitude = latitude;
        Longitude = longitude;
        CreatedAt = createdAt;
    }
}
