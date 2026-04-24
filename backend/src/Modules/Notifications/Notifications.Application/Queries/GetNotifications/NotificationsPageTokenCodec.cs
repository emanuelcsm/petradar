using Notifications.Application.Exceptions;
using System.Text;
using System.Text.Json;

namespace Notifications.Application.Queries.GetNotifications;

internal static class NotificationsPageTokenCodec
{
    public static NotificationsCursor? Decode(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var bytes = Convert.FromBase64String(token);
            var json = Encoding.UTF8.GetString(bytes);
            var payload = JsonSerializer.Deserialize<NotificationsPageTokenPayload>(json);

            if (payload is null || payload.CreatedAtTicksUtc <= 0 || string.IsNullOrWhiteSpace(payload.Id))
                throw new InvalidNotificationsPageTokenException();

            var createdAt = new DateTime(payload.CreatedAtTicksUtc, DateTimeKind.Utc);
            return new NotificationsCursor(createdAt, payload.Id);
        }
        catch (FormatException)
        {
            throw new InvalidNotificationsPageTokenException();
        }
        catch (JsonException)
        {
            throw new InvalidNotificationsPageTokenException();
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new InvalidNotificationsPageTokenException();
        }
    }

    public static string Encode(DateTime createdAtUtc, string id)
    {
        var payload = new NotificationsPageTokenPayload(
            CreatedAtTicksUtc: createdAtUtc.ToUniversalTime().Ticks,
            Id: id);

        var json = JsonSerializer.Serialize(payload);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }
}

internal sealed record NotificationsCursor(DateTime CreatedAtUtc, string Id);

internal sealed record NotificationsPageTokenPayload(long CreatedAtTicksUtc, string Id);
