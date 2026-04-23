using MediatR;

namespace Media.Application.Commands.UploadMedia;

public sealed record UploadMediaCommand(
    string UploadedBy,
    UploadMediaFileDto File) : IRequest<UploadMediaResult>;
