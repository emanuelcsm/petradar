using MediatR;

namespace Animals.Application.Queries.GetAnimalsByLocation;

public sealed record GetAnimalsByLocationQuery(
    double Latitude,
    double Longitude,
    double RadiusKm) : IRequest<IReadOnlyList<GetAnimalsByLocationResult>>;