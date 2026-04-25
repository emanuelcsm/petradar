using Identity.Application.Commands.Logout;
using Identity.Application.Interfaces.Auth;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace Identity.Application.Tests.Commands;

public sealed class LogoutCommandHandlerTests
{
    private readonly ITokenBlacklist _tokenBlacklist = Substitute.For<ITokenBlacklist>();

    private LogoutCommandHandler CreateHandler() => new(_tokenBlacklist);

    [Fact]
    public async Task Handle_WhenRemainingLifetimeIsPositive_AddsJtiToBlacklist()
    {
        var jti = "test-jti-id";
        var remaining = TimeSpan.FromMinutes(30);
        var command = new LogoutCommand(jti, remaining);

        await CreateHandler().Handle(command, CancellationToken.None);

        await _tokenBlacklist.Received(1).AddAsync(jti, remaining, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenRemainingLifetimeIsZero_DoesNotCallBlacklist()
    {
        var command = new LogoutCommand("test-jti-id", TimeSpan.Zero);

        await CreateHandler().Handle(command, CancellationToken.None);

        await _tokenBlacklist.DidNotReceive().AddAsync(Arg.Any<string>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRemainingLifetimeIsNegative_DoesNotCallBlacklist()
    {
        var command = new LogoutCommand("test-jti-id", TimeSpan.FromSeconds(-1));

        await CreateHandler().Handle(command, CancellationToken.None);

        await _tokenBlacklist.DidNotReceive().AddAsync(Arg.Any<string>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }
}
