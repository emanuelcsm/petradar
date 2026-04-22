using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class InvalidNearbySearchPageTokenException : DomainException
{
    public const string Code = "INVALID_NEARBY_SEARCH_PAGE_TOKEN";

    public InvalidNearbySearchPageTokenException()
        : base(Code, "Nearby search nextPageToken is invalid.")
    {
    }
}