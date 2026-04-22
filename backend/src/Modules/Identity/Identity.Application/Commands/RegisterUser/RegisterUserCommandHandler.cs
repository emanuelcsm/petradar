using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Security;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions.Authentication;
using MediatR;
using PetRadar.SharedKernel.Events;

namespace Identity.Application.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            throw new EmailAlreadyRegisteredException(request.Email);

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = User.Create(
            email: request.Email,
            name: request.Name,
            passwordHash: passwordHash);

        await _userRepository.SaveAsync(user, cancellationToken);

        var domainEvents = user.CollectDomainEvents();
        await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
