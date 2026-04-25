namespace Animals.Application.Cache;

internal static class NearbyAnimalsCacheKey
{
    private const int RegionPrecision = 1; // ~11 km tiles

    internal static (double Lat, double Lng) GetRegion(double latitude, double longitude)
        => (Math.Round(latitude, RegionPrecision), Math.Round(longitude, RegionPrecision));

    internal static string ForQuery(
        double regionLat,
        double regionLng,
        string version,
        double radiusKm,
        int pageSize,
        string? pageToken)
        => $"animals:nearby:{regionLat:F1}:{regionLng:F1}:{version}:{radiusKm}:{pageSize}:{pageToken ?? "first"}";

    internal static string ForRegionVersion(double regionLat, double regionLng)
        => $"animals:region_version:{regionLat:F1}:{regionLng:F1}";
}
