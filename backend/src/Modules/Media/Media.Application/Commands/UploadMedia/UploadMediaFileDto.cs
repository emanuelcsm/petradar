namespace Media.Application.Commands.UploadMedia;

public sealed record UploadMediaFileDto(
    Stream Content,
    string FileName,
    string ContentType);
