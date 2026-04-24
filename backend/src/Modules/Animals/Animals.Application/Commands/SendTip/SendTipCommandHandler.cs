using Animals.Application.Interfaces;
using Animals.Domain.Exceptions;
using MediatR;
using PetRadar.SharedKernel.Events;

namespace Animals.Application.Commands.SendTip;

public sealed class SendTipCommandHandler : IRequestHandler<SendTipCommand>
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public SendTipCommandHandler(
        IAnimalRepository animalRepository,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _animalRepository      = animalRepository;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(SendTipCommand request, CancellationToken cancellationToken)
    {
        var animalPost = await _animalRepository.GetByIdAsync(request.AnimalId, cancellationToken);
        if (animalPost is null)
            throw new AnimalPostNotFoundException(request.AnimalId);

        if (string.Equals(request.SenderId, animalPost.UserId, StringComparison.Ordinal))
            throw new TipSelfPostForbiddenException(request.AnimalId, request.SenderId);

        animalPost.RegisterTip(request.SenderName, request.Message);

        await _animalRepository.SaveAsync(animalPost, cancellationToken);

        var domainEvents = animalPost.CollectDomainEvents();
        await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
