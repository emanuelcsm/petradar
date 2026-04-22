namespace Animals.Application.Interfaces;

public interface IMediaValidator
{
    Task<bool> ExistsAsync(string mediaId, CancellationToken cancellationToken = default);
}
