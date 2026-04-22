using MediatR;

namespace Identity.Application.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<LoginResult>;
