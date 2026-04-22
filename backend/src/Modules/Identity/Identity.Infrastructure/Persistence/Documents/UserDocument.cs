using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Identity.Domain.Entities;
using PetRadar.SharedKernel.ValueObjects;

namespace Identity.Infrastructure.Persistence.Documents;

[BsonIgnoreExtraElements]
internal sealed class UserDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("email")]
    public string Email { get; set; } = default!;

    [BsonElement("emailNormalized")]
    public string EmailNormalized { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = default!;

    [BsonElement("alertLocation")]
    public GeoLocation? AlertLocation { get; set; }

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    public static UserDocument FromDomain(User user)
    {
        return new UserDocument
        {
            Id = user.Id,
            Email = user.Email.Value,
            EmailNormalized = user.Email.Value,
            Name = user.Name,
            PasswordHash = user.PasswordHash,
            AlertLocation = user.AlertLocation,
            CreatedAt = user.CreatedAt
        };
    }

    public User ToDomain()
    {
        return User.Rehydrate(
            id: Id,
            email: Email,
            name: Name,
            passwordHash: PasswordHash,
            alertLocation: AlertLocation,
            createdAt: CreatedAt);
    }
}
