using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class TipSelfPostForbiddenException : ForbiddenException
{
    public const string Code = "ANIMAL_TIP_SELF_POST_FORBIDDEN";

    public TipSelfPostForbiddenException(string animalPostId, string senderId)
        : base(Code, $"User '{senderId}' cannot send a tip to their own animal post '{animalPostId}'.") { }
}
