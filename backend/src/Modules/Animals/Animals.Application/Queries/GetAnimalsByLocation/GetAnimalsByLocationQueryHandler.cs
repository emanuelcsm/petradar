using System.Text.Json;
using Animals.Application.Cache;
using Animals.Application.Integration.Interfaces;
using Animals.Application.Interfaces;
using Animals.Domain.Exceptions;
using MediatR;
using PetRadar.SharedKernel.Pagination;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Application.Queries.GetAnimalsByLocation;

public sealed class GetAnimalsByLocationQueryHandler
    : IRequestHandler<GetAnimalsByLocationQuery, CursorPage<GetAnimalsByLocationResult>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    private readonly IAnimalRepository _animalRepository;
    private readonly IKnownMediaRepository _knownMediaRepository;
    private readonly IAnimalListingCache _cache;

    public GetAnimalsByLocationQueryHandler(
        IAnimalRepository animalRepository,
        IKnownMediaRepository knownMediaRepository,
        IAnimalListingCache cache)
    {
        _animalRepository = animalRepository;
        _knownMediaRepository = knownMediaRepository;
        _cache = cache;
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

        var (regionLat, regionLng) = NearbyAnimalsCacheKey.GetRegion(request.Latitude, request.Longitude);
        var versionKey = NearbyAnimalsCacheKey.ForRegionVersion(regionLat, regionLng);
        var version = await _cache.GetRegionVersionAsync(versionKey, cancellationToken);
        var cacheKey = NearbyAnimalsCacheKey.ForQuery(regionLat, regionLng, version, request.RadiusKm, request.PageSize, request.NextPageToken);

        var cached = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            var cachedPage = JsonSerializer.Deserialize<CursorPage<GetAnimalsByLocationResult>>(cached);
            if (cachedPage is not null)
                return cachedPage;
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

        var pageMediaIds = pageItems
            .SelectMany(animal => animal.MediaIds)
            .Distinct()
            .ToList();

        var knownMedia = await _knownMediaRepository.GetByIdsAsync(pageMediaIds, cancellationToken);

        var knownMediaById = knownMedia
            .ToDictionary(media => media.MediaId, media => media.PublicUrl, StringComparer.Ordinal);

        var data = pageItems
            .Select(animal => new GetAnimalsByLocationResult(
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
                MediaIds: animal.MediaIds,
                CreatedAt: animal.CreatedAt))
            .ToList();

        var page = new CursorPage<GetAnimalsByLocationResult>(
            Data: data,
            NextPageToken: nextPageToken,
            HasNextPage: hasNextPage);

        await _cache.SetAsync(cacheKey, JsonSerializer.Serialize(page), CacheTtl, cancellationToken);

        return page;
    }
}
