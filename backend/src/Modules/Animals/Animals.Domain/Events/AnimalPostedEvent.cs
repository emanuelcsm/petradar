using PetRadar.SharedKernel.Events;

namespace Animals.Domain.Events;

public sealed record AnimalPostedEvent : DomainEvent
{
    public string AnimalPostId { get; init; }
    public string UserId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime CreatedAt { get; init; }

    public AnimalPostedEvent(
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