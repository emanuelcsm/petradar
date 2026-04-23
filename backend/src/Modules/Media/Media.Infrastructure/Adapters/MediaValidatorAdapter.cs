using Animals.Application.Interfaces;
using Media.Application.Interfaces.Persistence;

namespace Media.Infrastructure.Adapters;

internal sealed class MediaValidatorAdapter : IMediaValidator
{
    private readonly IMediaRepository _mediaRepository;

    public MediaValidatorAdapter(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public Task<bool> ExistsAsync(string mediaId, CancellationToken cancellationToken = default)
    {
        return _mediaRepository.ExistsByIdAsync(mediaId, cancellationToken);
    }
}
