using PetRadar.SharedKernel.Exceptions;

namespace Identity.Domain.Exceptions.Authentication;

public sealed class EmailAlreadyRegisteredException : ConflictException
{
    public const string Code = "EMAIL_ALREADY_REGISTERED";

    public EmailAlreadyRegisteredException(string email)
        : base(Code, $"The email '{email}' is already registered.")
    {
    }
}
