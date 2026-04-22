using PetRadar.SharedKernel.Events;

namespace Animals.Domain.Events;

public sealed record AnimalFoundEvent : DomainEvent
{
    public string AnimalPostId { get; init; }
    public string UserId { get; init; }
    public DateTime FoundAt { get; init; }

    public AnimalFoundEvent(string animalPostId, string userId, DateTime foundAt)
    {
        AnimalPostId = animalPostId;
        UserId = userId;
        FoundAt = foundAt;
    }
}