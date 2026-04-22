using Animals.Domain.Exceptions;
using System.Text;
using System.Text.Json;

namespace Animals.Application.Queries.GetAnimalsByLocation;

internal static class NearbyAnimalsPageTokenCodec
{
    public static NearbyAnimalsCursor? Decode(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var bytes = Convert.FromBase64String(token);
            var json = Encoding.UTF8.GetString(bytes);
            var payload = JsonSerializer.Deserialize<NearbyAnimalsPageTokenPayload>(json);

            if (payload is null || payload.CreatedAtTicksUtc <= 0 || string.IsNullOrWhiteSpace(payload.Id))
                throw new InvalidNearbySearchPageTokenException();

            var createdAt = new DateTime(payload.CreatedAtTicksUtc, DateTimeKind.Utc);
            return new NearbyAnimalsCursor(createdAt, payload.Id);
        }
        catch (FormatException)
        {
            throw new InvalidNearbySearchPageTokenException();
        }
        catch (JsonException)
        {
            throw new InvalidNearbySearchPageTokenException();
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new InvalidNearbySearchPageTokenException();
        }
    }

    public static string Encode(DateTime createdAtUtc, string id)
    {
        var payload = new NearbyAnimalsPageTokenPayload(
            CreatedAtTicksUtc: createdAtUtc.ToUniversalTime().Ticks,
            Id: id);

        var json = JsonSerializer.Serialize(payload);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }
}

internal sealed record NearbyAnimalsCursor(DateTime CreatedAtUtc, string Id);

internal sealed record NearbyAnimalsPageTokenPayload(long CreatedAtTicksUtc, string Id);