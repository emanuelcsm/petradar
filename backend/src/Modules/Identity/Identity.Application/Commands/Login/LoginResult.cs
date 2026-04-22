namespace Identity.Application.Commands.Login;

public sealed record LoginResult(
    string Token,
    string UserId,
    string Email,
    string Name);
