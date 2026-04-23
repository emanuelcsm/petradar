using Media.Application.Commands.UploadMedia;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetRadar.API.Contracts.Media.Requests;
using PetRadar.API.Contracts.Media.Responses;
using PetRadar.API.Infrastructure.Auth;
using PetRadar.API.Infrastructure.Responses;
using PetRadar.API.Infrastructure.Validation.Media;

namespace PetRadar.API.Controllers;

[ApiController]
[Route("api/media")]
[Authorize]
public sealed class MediaController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMediaUploadRequestValidator _mediaUploadRequestValidator;

    public MediaController(
        ISender sender,
        IMediaUploadRequestValidator mediaUploadRequestValidator)
    {
        _sender = sender;
        _mediaUploadRequestValidator = mediaUploadRequestValidator;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
        [FromForm] UploadMediaRequest request,
        CancellationToken cancellationToken)
    {
        _mediaUploadRequestValidator.Validate(request.File);

        var userId = User.GetRequiredUserId();

        var command = new UploadMediaCommand( 
            UploadedBy: userId,
            File: new UploadMediaFileDto(
                Content: request.File.OpenReadStream(),
                FileName: request.File.FileName,
                ContentType: request.File.ContentType));

        var result = await _sender.Send(command, cancellationToken);

        var response = new UploadMediaResponse(result.MediaId);
        return Ok(response.ToResponse());
    }
}
