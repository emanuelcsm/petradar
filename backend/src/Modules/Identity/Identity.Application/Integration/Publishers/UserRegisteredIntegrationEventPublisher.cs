using Identity.Domain.Events;
using MediatR;
using PetRadar.IntegrationEvents.Identity;

namespace Identity.Application.Integration.Publishers;

public sealed class UserRegisteredIntegrationEventPublisher : INotificationHandler<UserRegisteredEvent>
{
    private readonly IMediator _mediator;

    public UserRegisteredIntegrationEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new UserRegisteredIntegrationEvent(
            notification.UserId,
            notification.Email,
            notification.Name,
            DateTime.UtcNow);

        await _mediator.Publish(integrationEvent, cancellationToken);
    }
}
