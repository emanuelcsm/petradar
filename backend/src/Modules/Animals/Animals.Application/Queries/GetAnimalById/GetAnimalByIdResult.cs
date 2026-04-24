using Animals.Application.Queries.GetAnimalsByLocation;

namespace Animals.Application.Queries.GetAnimalById;

public sealed record GetAnimalByIdResult(
    string Id,
    string UserId,
    string Description,
    string Status,
    double Latitude,
    double Longitude,
    IReadOnlyList<AnimalMediaResult> Media,
    DateTime CreatedAt);
