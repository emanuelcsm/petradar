using MediatR;
using PetRadar.SharedKernel.Pagination;

namespace Animals.Application.Queries.GetAnimalsByLocation;

public sealed record GetAnimalsByLocationQuery(
    double Latitude,
    double Longitude,
    double RadiusKm,
    int PageSize,
    int MaxPageSize,
    string? NextPageToken) : IRequest<CursorPage<GetAnimalsByLocationResult>>;