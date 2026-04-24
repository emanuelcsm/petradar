using Identity.Application.Commands.RegisterUser;
using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Security;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions.Authentication;
using NSubstitute;
using PetRadar.SharedKernel.Events;

namespace Identity.Application.Tests.Commands;

public sealed class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IDomainEventDispatcher _dispatcher = Substitute.For<IDomainEventDispatcher>();

    private RegisterUserCommandHandler CreateHandler() =>
        new(_userRepository, _passwordHasher, _dispatcher);

    [Fact]
    public async Task Handle_WithNewEmail_SavesUserAndDispatchesDomainEvents()
    {
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed-password");

        var command = new RegisterUserCommand("novo@email.com", "João", "senha123");
        await CreateHandler().Handle(command, CancellationToken.None);

        await _userRepository.Received(1).SaveAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _dispatcher.Received(1).DispatchAsync(
            Arg.Any<IEnumerable<DomainEvent>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ThrowsEmailAlreadyRegisteredException()
    {
        var existingUser = User.Create("existente@email.com", "Existente", "hash");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(existingUser);

        var command = new RegisterUserCommand("existente@email.com", "Outro", "senha123");

        await Assert.ThrowsAsync<EmailAlreadyRegisteredException>(
            () => CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CallsPasswordHasherWithRawPassword()
    {
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed-password");

        var command = new RegisterUserCommand("novo@email.com", "João", "senha-secreta");
        await CreateHandler().Handle(command, CancellationToken.None);

        _passwordHasher.Received(1).Hash("senha-secreta");
    }
}
