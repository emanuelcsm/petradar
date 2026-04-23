namespace PetRadar.API.Contracts.Animals.Responses;

public sealed record NearbyAnimalResponse(
    string Id,
    string UserId,
    string Description,
    string Status,
    double Latitude,
    double Longitude,
    IReadOnlyList<AnimalMediaResponse> Media,
    DateTime CreatedAt);