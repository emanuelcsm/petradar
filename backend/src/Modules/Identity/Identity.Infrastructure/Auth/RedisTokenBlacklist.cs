using Identity.Application.Interfaces.Auth;
using Microsoft.Extensions.Caching.Distributed;

namespace Identity.Infrastructure.Auth;

public sealed class RedisTokenBlacklist : ITokenBlacklist
{
    private readonly IDistributedCache _cache;

    public RedisTokenBlacklist(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(string jti, TimeSpan ttl, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        await _cache.SetStringAsync(BuildKey(jti), "1", options, ct);
    }

    public async Task<bool> IsRevokedAsync(string jti, CancellationToken ct = default)
    {
        return await _cache.GetStringAsync(BuildKey(jti), ct) is not null;
    }

    private static string BuildKey(string jti) => $"jti_blacklist:{jti}";
}
