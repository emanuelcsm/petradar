using Animals.Application.Cache;
using Microsoft.Extensions.Caching.Distributed;

namespace Animals.Infrastructure.Cache;

internal sealed class RedisAnimalListingCache : IAnimalListingCache
{
    private readonly IDistributedCache _distributedCache;

    public RedisAnimalListingCache(IDistributedCache distributedCache)
        => _distributedCache = distributedCache;

    public async Task<string?> GetAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetAsync(string cacheKey, string value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };
            await _distributedCache.SetStringAsync(cacheKey, value, options, cancellationToken);
        }
        catch { }
    }

    public async Task<string> GetRegionVersionAsync(string regionVersionKey, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _distributedCache.GetStringAsync(regionVersionKey, cancellationToken) ?? "0";
        }
        catch
        {
            return "0";
        }
    }

    public async Task InvalidateRegionAsync(string regionVersionKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var version = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            };
            await _distributedCache.SetStringAsync(regionVersionKey, version, options, cancellationToken);
        }
        catch { }
    }
}
