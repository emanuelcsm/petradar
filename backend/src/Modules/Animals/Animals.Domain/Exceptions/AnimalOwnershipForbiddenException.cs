using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class AnimalOwnershipForbiddenException : ForbiddenException
{
    public const string Code = "ANIMAL_OWNERSHIP_FORBIDDEN";

    public AnimalOwnershipForbiddenException(string animalPostId, string requesterUserId)
        : base(Code, $"User '{requesterUserId}' is not allowed to mark animal post '{animalPostId}' as found.")
    {
    }
}
