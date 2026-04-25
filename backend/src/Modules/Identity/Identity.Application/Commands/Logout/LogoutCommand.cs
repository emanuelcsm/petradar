using MediatR;

namespace Identity.Application.Commands.Logout;

public sealed record LogoutCommand(
    string Jti,
    TimeSpan RemainingLifetime) : IRequest;
