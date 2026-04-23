namespace Animals.Application.Queries.GetAnimalsByLocation;

public sealed record GetAnimalsByLocationResult(
    string Id,
    string UserId,
    string Description,
    string Status,
    double Latitude,
    double Longitude,
    IReadOnlyList<AnimalMediaResult> Media,
    IReadOnlyList<string> MediaIds,
    DateTime CreatedAt);