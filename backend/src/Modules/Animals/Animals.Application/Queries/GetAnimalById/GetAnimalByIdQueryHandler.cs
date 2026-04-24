using Animals.Application.Integration.Interfaces;
using Animals.Application.Interfaces;
using Animals.Application.Queries.GetAnimalsByLocation;
using Animals.Domain.Exceptions;
using MediatR;

namespace Animals.Application.Queries.GetAnimalById;

public sealed class GetAnimalByIdQueryHandler : IRequestHandler<GetAnimalByIdQuery, GetAnimalByIdResult>
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IKnownMediaRepository _knownMediaRepository;

    public GetAnimalByIdQueryHandler(
        IAnimalRepository animalRepository,
        IKnownMediaRepository knownMediaRepository)
    {
        _animalRepository = animalRepository;
        _knownMediaRepository = knownMediaRepository;
    }

    public async Task<GetAnimalByIdResult> Handle(
        GetAnimalByIdQuery request,
        CancellationToken cancellationToken)
    {
        var animal = await _animalRepository.GetByIdAsync(request.AnimalPostId, cancellationToken);
        if (animal is null)
            throw new AnimalPostNotFoundException(request.AnimalPostId);

        var knownMedia = await _knownMediaRepository.GetByIdsAsync(animal.MediaIds, cancellationToken);
        var knownMediaById = knownMedia.ToDictionary(
            m => m.MediaId,
            m => m.PublicUrl,
            StringComparer.Ordinal);

        return new GetAnimalByIdResult(
            Id: animal.Id,
            UserId: animal.UserId,
            Description: animal.Description,
            Status: animal.Status.Value,
            Latitude: animal.Location.Latitude,
            Longitude: animal.Location.Longitude,
            Media: animal.MediaIds
                .Where(mediaId => knownMediaById.ContainsKey(mediaId))
                .Select(mediaId => new AnimalMediaResult(
                    MediaId: mediaId,
                    Url: knownMediaById[mediaId]))
                .ToList(),
            CreatedAt: animal.CreatedAt);
    }
}
