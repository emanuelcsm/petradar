namespace Identity.Application.Interfaces.Auth;

public interface ITokenBlacklist
{
    Task AddAsync(string jti, TimeSpan ttl, CancellationToken ct = default);
    Task<bool> IsRevokedAsync(string jti, CancellationToken ct = default);
}
