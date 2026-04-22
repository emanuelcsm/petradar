using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using Animals.Domain.Entities;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Infrastructure.Persistence.Documents;

[BsonIgnoreExtraElements]
internal sealed class AnimalPostDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("userId")]
    public string UserId { get; set; } = default!;

    [BsonElement("description")]
    public string Description { get; set; } = default!;

    [BsonElement("status")]
    public string Status { get; set; } = default!;

    [BsonElement("location")]
    public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; } = default!;

    [BsonElement("mediaIds")]
    public List<string> MediaIds { get; set; } = [];

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    public static AnimalPostDocument FromDomain(AnimalPost animalPost)
    {
        return new AnimalPostDocument
        {
            Id = animalPost.Id,
            UserId = animalPost.UserId,
            Description = animalPost.Description,
            Status = animalPost.Status.Value,
            Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                new GeoJson2DGeographicCoordinates(
                    animalPost.Location.Longitude,
                    animalPost.Location.Latitude)),
            MediaIds = animalPost.MediaIds.ToList(),
            CreatedAt = animalPost.CreatedAt
        };
    }

    public AnimalPost ToDomain()
    {
        var location = new GeoLocation(
            Location.Coordinates.Latitude,
            Location.Coordinates.Longitude);

        return AnimalPost.Rehydrate(
            id: Id,
            userId: UserId,
            description: Description,
            status: Status,
            location: location,
            mediaIds: MediaIds,
            createdAt: CreatedAt);
    }
}