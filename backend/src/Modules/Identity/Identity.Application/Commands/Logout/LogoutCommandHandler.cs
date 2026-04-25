using Identity.Application.Interfaces.Auth;
using MediatR;

namespace Identity.Application.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly ITokenBlacklist _tokenBlacklist;

    public LogoutCommandHandler(ITokenBlacklist tokenBlacklist)
    {
        _tokenBlacklist = tokenBlacklist;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (request.RemainingLifetime > TimeSpan.Zero)
            await _tokenBlacklist.AddAsync(request.Jti, request.RemainingLifetime, cancellationToken);
    }
}
