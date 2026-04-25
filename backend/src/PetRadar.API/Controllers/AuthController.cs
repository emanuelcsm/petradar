using Identity.Application.Commands.Login;
using Identity.Application.Commands.Logout;
using Identity.Application.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetRadar.API.Contracts.Identity.Requests;
using PetRadar.API.Contracts.Identity.Responses;
using PetRadar.API.Infrastructure.Auth;
using PetRadar.API.Infrastructure.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PetRadar.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [AllowAnonymous]
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
    [AllowAnonymous]
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

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var jti = HttpContext.User.GetJti();

        if (jti is not null)
        {
            var remainingLifetime = TimeSpan.Zero;

            if (long.TryParse(HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Exp), out var expUnix))
            {
                var expiry = DateTimeOffset.FromUnixTimeSeconds(expUnix);
                var remaining = expiry - DateTimeOffset.UtcNow;
                if (remaining > TimeSpan.Zero)
                    remainingLifetime = remaining;
            }

            await _sender.Send(new LogoutCommand(jti, remainingLifetime), cancellationToken);
        }

        return NoContent();
    }
}
