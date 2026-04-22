namespace Identity.Application.Interfaces.Auth;

public interface IJwtTokenService
{
    string Generate(string userId, string email, string name);
}
