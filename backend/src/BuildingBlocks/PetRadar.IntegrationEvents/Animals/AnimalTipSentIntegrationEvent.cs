namespace PetRadar.IntegrationEvents.Animals;

public sealed record AnimalTipSentIntegrationEvent : IntegrationEvent
{
    public string AnimalPostId { get; init; }
    public string OwnerId { get; init; }
    public string SenderName { get; init; }
    public string Message { get; init; }

    public AnimalTipSentIntegrationEvent(
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
