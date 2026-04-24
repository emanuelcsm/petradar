using Animals.Application.Commands.CreateAnimalPost;
using Animals.Application.Commands.MarkAsFound;
using Animals.Application.Queries.GetAnimalById;
using Animals.Application.Queries.GetAnimalsByLocation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetRadar.API.Contracts.Animals.Requests;
using PetRadar.API.Contracts.Animals.Responses;
using PetRadar.API.Infrastructure.Auth;
using PetRadar.API.Infrastructure.Options;
using PetRadar.API.Infrastructure.Responses;
using PetRadar.SharedKernel.ValueObjects;

namespace PetRadar.API.Controllers;

[ApiController]
[Route("api/animals")]
public sealed class AnimalsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly CursorPaginationOptions _cursorPaginationOptions;

    public AnimalsController(
        ISender sender,
        IOptions<CursorPaginationOptions> cursorPaginationOptions)
    {
        _sender = sender;
        _cursorPaginationOptions = cursorPaginationOptions.Value;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] CreateAnimalPostRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();

        var command = new CreateAnimalPostCommand(
            UserId: userId,
            Description: request.Description,
            Location: new GeoLocation(request.Latitude, request.Longitude),
            MediaIds: request.MediaIds);

        await _sender.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var query = new GetAnimalByIdQuery(AnimalPostId: id);
        var result = await _sender.Send(query, cancellationToken);

        var response = new NearbyAnimalResponse(
            Id: result.Id,
            UserId: result.UserId,
            Description: result.Description,
            Status: result.Status,
            Latitude: result.Latitude,
            Longitude: result.Longitude,
            Media: result.Media
                .Select(m => new AnimalMediaResponse(MediaId: m.MediaId, Url: m.Url))
                .ToList(),
            CreatedAt: result.CreatedAt);

        return Ok(response.ToResponse());
    }

    [HttpGet("nearby")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] double radius,
        [FromQuery] string? nextPageToken,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var effectivePageSize = pageSize ?? _cursorPaginationOptions.DefaultPageSize;

        var query = new GetAnimalsByLocationQuery(
            Latitude: lat,
            Longitude: lng,
            RadiusKm: radius,
            PageSize: effectivePageSize,
            MaxPageSize: _cursorPaginationOptions.MaxPageSize,
            NextPageToken: nextPageToken);

        var result = await _sender.Send(query, cancellationToken);

        var response = result.Data
            .Select(item => new NearbyAnimalResponse(
                Id: item.Id,
                UserId: item.UserId,
                Description: item.Description,
                Status: item.Status,
                Latitude: item.Latitude,
                Longitude: item.Longitude,
                Media: item.Media
                    .Select(media => new AnimalMediaResponse(
                        MediaId: media.MediaId,
                        Url: media.Url))
                    .ToList(),
                CreatedAt: item.CreatedAt))
            .ToList();

        return Ok(response.ToPagedResponse(
            nextPageToken: result.NextPageToken,
            hasNextPage: result.HasNextPage));
    }

    [HttpPatch("{id}/found")]
    [Authorize]
    public async Task<IActionResult> MarkAsFound(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var requesterUserId = User.GetRequiredUserId();

        var command = new MarkAsFoundCommand(
            AnimalPostId: id,
            RequesterUserId: requesterUserId);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
}