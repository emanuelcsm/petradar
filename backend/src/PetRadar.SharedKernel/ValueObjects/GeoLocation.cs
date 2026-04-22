namespace PetRadar.SharedKernel.ValueObjects;

/// <summary>
/// Represents a geographic coordinate pair. Immutable.
/// Latitude: [-90, 90], Longitude: [-180, 180].
/// </summary>
public sealed record GeoLocation
{
    public double Latitude { get; }
    public double Longitude { get; }

    public GeoLocation(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        Latitude = latitude;
        Longitude = longitude;
    }

    public override string ToString() => $"({Latitude}, {Longitude})";
}
