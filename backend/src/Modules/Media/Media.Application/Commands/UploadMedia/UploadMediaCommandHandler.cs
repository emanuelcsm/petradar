using Media.Application.Interfaces.Persistence;
using Media.Application.Interfaces.Storage;
using Media.Domain.Entities;
using MediatR;

namespace Media.Application.Commands.UploadMedia;

public sealed class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, UploadMediaResult>
{
    private readonly IMediaStorage _mediaStorage;
    private readonly IMediaRepository _mediaRepository;

    public UploadMediaCommandHandler(
        IMediaStorage mediaStorage,
        IMediaRepository mediaRepository)
    {
        _mediaStorage = mediaStorage;
        _mediaRepository = mediaRepository;
    }

    public async Task<UploadMediaResult> Handle(
        UploadMediaCommand request,
        CancellationToken cancellationToken)
    {
        var storagePath = await _mediaStorage.UploadAsync(
            request.File.Content,
            request.File.FileName,
            request.File.ContentType,
            cancellationToken);

        var mediaFile = MediaFile.Create(
            request.File.FileName,
            request.File.ContentType,
            storagePath,
            request.UploadedBy);

        try
        {
            await _mediaRepository.SaveAsync(mediaFile, cancellationToken);
        }
        catch
        {
            try
            {
                await _mediaStorage.DeleteAsync(storagePath, cancellationToken);
            }
            catch
            {
                // Preserve the original persistence failure.
            }

            throw;
        }

        return new UploadMediaResult(mediaFile.Id);
    }
}
