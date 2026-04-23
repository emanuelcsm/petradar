using Media.Application.Interfaces.Persistence;
using Media.Application.Interfaces.Storage;
using Media.Domain.Entities;
using MediatR;
using PetRadar.SharedKernel.Events;

namespace Media.Application.Commands.UploadMedia;

public sealed class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, UploadMediaResult>
{
    private readonly IMediaStorage _mediaStorage;
    private readonly IMediaRepository _mediaRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public UploadMediaCommandHandler(
        IMediaStorage mediaStorage,
        IMediaRepository mediaRepository,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _mediaStorage = mediaStorage;
        _mediaRepository = mediaRepository;
        _domainEventDispatcher = domainEventDispatcher;
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

            var domainEvents = mediaFile.CollectDomainEvents();
            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
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
