using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class InvalidNearbySearchPageSizeException : DomainException
{
    public const string Code = "INVALID_NEARBY_SEARCH_PAGE_SIZE";

    public InvalidNearbySearchPageSizeException(int pageSize)
        : base(Code, $"Nearby search page size must be greater than zero. Received '{pageSize}'.")
    {
    }

    public InvalidNearbySearchPageSizeException(int pageSize, int maxPageSize)
        : base(Code, $"Nearby search page size must be between 1 and '{maxPageSize}'. Received '{pageSize}'.")
    {
    }
}