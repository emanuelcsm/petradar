using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class InvalidNearbySearchRadiusException : DomainException
{
    public const string Code = "INVALID_NEARBY_SEARCH_RADIUS";

    public InvalidNearbySearchRadiusException(double radiusKm)
        : base(Code, $"Nearby search radius must be greater than zero. Received '{radiusKm}'.")
    {
    }
}