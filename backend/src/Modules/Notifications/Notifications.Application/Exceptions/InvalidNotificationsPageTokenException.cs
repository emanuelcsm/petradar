using PetRadar.SharedKernel.Exceptions;

namespace Notifications.Application.Exceptions;

public sealed class InvalidNotificationsPageTokenException : DomainException
{
    public const string Code = "INVALID_NOTIFICATIONS_PAGE_TOKEN";

    public InvalidNotificationsPageTokenException()
        : base(Code, "Notifications nextPageToken is invalid.")
    {
    }
}
