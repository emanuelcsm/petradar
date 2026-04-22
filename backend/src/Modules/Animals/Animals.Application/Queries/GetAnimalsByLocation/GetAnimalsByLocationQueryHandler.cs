using Animals.Application.Interfaces;
using Animals.Domain.Exceptions;
using MediatR;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Application.Queries.GetAnimalsByLocation;

public sealed class GetAnimalsByLocationQueryHandler
    : IRequestHandler<GetAnimalsByLocationQuery, IReadOnlyList<GetAnimalsByLocationResult>>
{
    private readonly IAnimalRepository _animalRepository;

    public GetAnimalsByLocationQueryHandler(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    public async Task<IReadOnlyList<GetAnimalsByLocationResult>> Handle(
        GetAnimalsByLocationQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RadiusKm <= 0)
            throw new InvalidNearbySearchRadiusException(request.RadiusKm);

        GeoLocation center;

        try
        {
            center = new GeoLocation(request.Latitude, request.Longitude);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new InvalidNearbySearchLocationException(request.Latitude, request.Longitude);
        }

        var animals = await _animalRepository.GetNearbyAsync(
            center,
            request.RadiusKm,
            cancellationToken);

        return animals
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
    }
}