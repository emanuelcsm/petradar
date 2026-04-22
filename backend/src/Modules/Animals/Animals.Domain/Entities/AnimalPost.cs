using Animals.Domain.Exceptions;
using Animals.Domain.Events;
using Animals.Domain.ValueObjects;
using PetRadar.SharedKernel.Entities;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Domain.Entities;

public sealed class AnimalPost : AggregateRoot
{
    public string UserId { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public AnimalStatus Status { get; private set; } = default!;
    public GeoLocation Location { get; private set; } = default!;
    public IReadOnlyList<string> MediaIds { get; private set; } = default!;
    public DateTime CreatedAt { get; private init; }

    private AnimalPost() { }

    private AnimalPost(
        string id,
        string userId,
        string description,
        AnimalStatus status,
        GeoLocation location,
        IReadOnlyList<string> mediaIds,
        DateTime createdAt) : base(id)
    {
        UserId = userId;
        Description = description;
        Status = status;
        Location = location;
        MediaIds = mediaIds;
        CreatedAt = createdAt;
    }

    public static AnimalPost Create(
        string userId,
        string description,
        GeoLocation location,
        IReadOnlyList<string>? mediaIds)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new InvalidAnimalUserIdException();

        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidAnimalDescriptionException("Animal description cannot be null or empty.");

        var normalizedDescription = description.Trim();

        if (normalizedDescription.Length < 10)
            throw new InvalidAnimalDescriptionException("Animal description must have at least 10 characters.");

        IReadOnlyList<string> normalizedMediaIds = mediaIds?.ToList().AsReadOnly() ?? [];

        var animalPost = new AnimalPost(
            Guid.NewGuid().ToString(),
            userId.Trim(),
            normalizedDescription,
            AnimalStatus.Lost(),
            location,
            normalizedMediaIds,
            DateTime.UtcNow);

        animalPost.AddDomainEvent(new AnimalPostedEvent(
            animalPost.Id,
            animalPost.UserId,
            animalPost.Location.Latitude,
            animalPost.Location.Longitude,
            animalPost.CreatedAt));

        return animalPost;
    }

    public void MarkAsFound()
    {
        if (Status == AnimalStatus.Found())
            throw new AnimalAlreadyFoundException(Id);

        Status = AnimalStatus.Found();
    }
}