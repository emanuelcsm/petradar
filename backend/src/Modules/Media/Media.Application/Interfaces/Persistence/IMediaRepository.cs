using Media.Domain.Entities;

namespace Media.Application.Interfaces.Persistence;

public interface IMediaRepository
{
    Task SaveAsync(MediaFile mediaFile, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(string mediaId, CancellationToken cancellationToken = default);
}
