using PetRadar.SharedKernel.Exceptions;

namespace Notifications.Application.Exceptions;

public sealed class InvalidNotificationsPageSizeException : DomainException
{
    public const string Code = "INVALID_NOTIFICATIONS_PAGE_SIZE";

    public InvalidNotificationsPageSizeException(int pageSize, int maxPageSize)
        : base(Code, $"Notifications pageSize must be between 1 and {maxPageSize}. Received: {pageSize}.")
    {
    }
}
