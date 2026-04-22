using Identity.Application.Commands.Login;
using Identity.Application.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetRadar.API.Contracts.Identity.Requests;
using PetRadar.API.Contracts.Identity.Responses;
using PetRadar.API.Infrastructure.Responses;

namespace PetRadar.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            Email: request.Email,
            Name: request.Name,
            Password: request.Password);

        await _sender.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            Email: request.Email,
            Password: request.Password);

        var result = await _sender.Send(command, cancellationToken);

        var response = new LoginResponse(
            Token: result.Token,
            UserId: result.UserId,
            Email: result.Email,
            Name: result.Name);

        return Ok(response.ToResponse());
    }
}
