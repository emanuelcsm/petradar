namespace PetRadar.IntegrationEvents.Animals;

public sealed record AnimalPostDeletedIntegrationEvent : IntegrationEvent
{
    public string AnimalPostId { get; init; }
    public string UserId { get; init; }
    public IReadOnlyList<string> MediaIds { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime DeletedAt { get; init; }

    public AnimalPostDeletedIntegrationEvent(
        string animalPostId,
        string userId,
        IReadOnlyList<string> mediaIds,
        double latitude,
        double longitude,
        DateTime deletedAt)
    {
        AnimalPostId = animalPostId;
        UserId = userId;
        MediaIds = mediaIds;
        Latitude = latitude;
        Longitude = longitude;
        DeletedAt = deletedAt;
    }
}
