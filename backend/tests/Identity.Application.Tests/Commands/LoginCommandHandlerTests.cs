using Identity.Application.Commands.Login;
using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Auth;
using Identity.Application.Interfaces.Security;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions.Authentication;
using NSubstitute;

namespace Identity.Application.Tests.Commands;

public sealed class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();

    private LoginCommandHandler CreateHandler() =>
        new(_userRepository, _passwordHasher, _jwtTokenService);

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsLoginResultWithToken()
    {
        var user = User.Create("joao@email.com", "João", "hash-correto");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        _jwtTokenService.Generate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("jwt-token");

        var command = new LoginCommand("joao@email.com", "senha");
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal("jwt-token", result.Token);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.Email.Value, result.Email);
        Assert.Equal(user.Name, result.Name);
    }

    [Fact]
    public async Task Handle_WithNonExistentEmail_ThrowsInvalidCredentialsException()
    {
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = new LoginCommand("naoexiste@email.com", "senha");

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsInvalidCredentialsException()
    {
        var user = User.Create("joao@email.com", "João", "hash-correto");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var command = new LoginCommand("joao@email.com", "senha-errada");

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => CreateHandler().Handle(command, CancellationToken.None));
    }
}
