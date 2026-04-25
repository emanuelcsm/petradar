using Animals.Domain.Events;
using MediatR;

namespace Animals.Application.Cache;

public sealed class AnimalListingCacheInvalidator
    : INotificationHandler<AnimalPostedEvent>,
      INotificationHandler<AnimalFoundEvent>
{
    private readonly IAnimalListingCache _cache;

    public AnimalListingCacheInvalidator(IAnimalListingCache cache)
        => _cache = cache;

    public Task Handle(AnimalPostedEvent notification, CancellationToken cancellationToken)
        => InvalidateAsync(notification.Latitude, notification.Longitude, cancellationToken);

    public Task Handle(AnimalFoundEvent notification, CancellationToken cancellationToken)
        => InvalidateAsync(notification.Latitude, notification.Longitude, cancellationToken);

    private async Task InvalidateAsync(double latitude, double longitude, CancellationToken cancellationToken)
    {
        var (regionLat, regionLng) = NearbyAnimalsCacheKey.GetRegion(latitude, longitude);
        var versionKey = NearbyAnimalsCacheKey.ForRegionVersion(regionLat, regionLng);
        await _cache.InvalidateRegionAsync(versionKey, cancellationToken);
    }
}
