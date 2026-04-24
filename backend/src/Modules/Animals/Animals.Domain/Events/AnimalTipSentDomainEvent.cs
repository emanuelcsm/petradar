using PetRadar.SharedKernel.Events;

namespace Animals.Domain.Events;

public sealed record AnimalTipSentDomainEvent : DomainEvent
{
    public string AnimalPostId { get; init; }
    public string OwnerId { get; init; }
    public string SenderName { get; init; }
    public string Message { get; init; }

    public AnimalTipSentDomainEvent(
        string animalPostId,
        string ownerId,
        string senderName,
        string message)
    {
        AnimalPostId = animalPostId;
        OwnerId      = ownerId;
        SenderName   = senderName;
        Message      = message;
    }
}
