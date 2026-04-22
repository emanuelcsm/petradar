using Animals.Application.Interfaces;
using Animals.Domain.Exceptions;
using MediatR;
using PetRadar.SharedKernel.Events;

namespace Animals.Application.Commands.MarkAsFound;

public sealed class MarkAsFoundCommandHandler : IRequestHandler<MarkAsFoundCommand>
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public MarkAsFoundCommandHandler(
        IAnimalRepository animalRepository,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _animalRepository = animalRepository;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(MarkAsFoundCommand request, CancellationToken cancellationToken)
    {
        var animalPost = await _animalRepository.GetByIdAsync(request.AnimalPostId, cancellationToken);
        if (animalPost is null)
            throw new AnimalPostNotFoundException(request.AnimalPostId);

        if (!string.Equals(animalPost.UserId, request.RequesterUserId, StringComparison.Ordinal))
            throw new AnimalOwnershipForbiddenException(request.AnimalPostId, request.RequesterUserId);

        animalPost.MarkAsFound();

        await _animalRepository.SaveAsync(animalPost, cancellationToken);

        var domainEvents = animalPost.CollectDomainEvents();
        await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
