using PetRadar.SharedKernel.Exceptions;

namespace Animals.Domain.Exceptions;

public sealed class InvalidNearbySearchLocationException : DomainException
{
    public const string Code = "INVALID_NEARBY_SEARCH_LOCATION";

    public InvalidNearbySearchLocationException(double latitude, double longitude)
        : base(Code, $"Nearby search coordinates are invalid. Received latitude '{latitude}' and longitude '{longitude}'.")
    {
    }
}