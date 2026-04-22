using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Auth;
using Identity.Application.Interfaces.Security;
using Identity.Domain.Exceptions.Authentication;
using MediatR;

namespace Identity.Application.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var token = _jwtTokenService.Generate(user.Id, user.Email.Value, user.Name);

        return new LoginResult(
            Token: token,
            UserId: user.Id,
            Email: user.Email.Value,
            Name: user.Name);
    }
}
