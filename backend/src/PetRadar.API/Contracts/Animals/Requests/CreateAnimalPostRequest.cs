namespace PetRadar.API.Contracts.Animals.Requests;

public sealed record CreateAnimalPostRequest(
    string Description,
    double Latitude,
    double Longitude,
    IReadOnlyList<string>? MediaIds);
