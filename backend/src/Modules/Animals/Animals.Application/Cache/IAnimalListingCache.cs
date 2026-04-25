namespace Animals.Application.Cache;

public interface IAnimalListingCache
{
    Task<string?> GetAsync(string cacheKey, CancellationToken cancellationToken = default);
    Task SetAsync(string cacheKey, string value, TimeSpan ttl, CancellationToken cancellationToken = default);
    Task<string> GetRegionVersionAsync(string regionVersionKey, CancellationToken cancellationToken = default);
    Task InvalidateRegionAsync(string regionVersionKey, CancellationToken cancellationToken = default);
}
