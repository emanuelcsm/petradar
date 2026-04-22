using Animals.Application.Interfaces;
using Animals.Domain.Exceptions;
using MediatR;
using PetRadar.SharedKernel.Pagination;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Application.Queries.GetAnimalsByLocation;

public sealed class GetAnimalsByLocationQueryHandler
    : IRequestHandler<GetAnimalsByLocationQuery, CursorPage<GetAnimalsByLocationResult>>
{
    private readonly IAnimalRepository _animalRepository;

    public GetAnimalsByLocationQueryHandler(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    public async Task<CursorPage<GetAnimalsByLocationResult>> Handle(
        GetAnimalsByLocationQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RadiusKm <= 0)
            throw new InvalidNearbySearchRadiusException(request.RadiusKm);

        if (request.PageSize <= 0 || request.PageSize > request.MaxPageSize)
            throw new InvalidNearbySearchPageSizeException(request.PageSize, request.MaxPageSize);

        GeoLocation center;

        try
        {
            center = new GeoLocation(request.Latitude, request.Longitude);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new InvalidNearbySearchLocationException(request.Latitude, request.Longitude);
        }

        var cursor = NearbyAnimalsPageTokenCodec.Decode(request.NextPageToken);

        var animalsSlice = await _animalRepository.GetNearbyAsync(
            center,
            request.RadiusKm,
            cursor?.CreatedAtUtc,
            cursor?.Id,
            request.PageSize,
            cancellationToken);

        var pageItems = animalsSlice.Items;
        var hasNextPage = animalsSlice.HasNextPage;

        var nextPageToken = hasNextPage
            ? NearbyAnimalsPageTokenCodec.Encode(
                pageItems[^1].CreatedAt,
                pageItems[^1].Id)
            : null;

        var data = pageItems
            .Select(animal => new GetAnimalsByLocationResult(
                Id: animal.Id,
                UserId: animal.UserId,
                Description: animal.Description,
                Status: animal.Status.Value,
                Latitude: animal.Location.Latitude,
                Longitude: animal.Location.Longitude,
                MediaIds: animal.MediaIds,
                CreatedAt: animal.CreatedAt))
            .ToList();

        return new CursorPage<GetAnimalsByLocationResult>(
            Data: data,
            NextPageToken: nextPageToken,
            HasNextPage: hasNextPage);
    }
}