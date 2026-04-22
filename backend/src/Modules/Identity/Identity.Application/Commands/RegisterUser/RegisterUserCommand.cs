using MediatR;

namespace Identity.Application.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Name,
    string Password) : IRequest;
