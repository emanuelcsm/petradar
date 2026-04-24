using Animals.Application.Interfaces;
using Animals.Domain.Exceptions;
using MediatR;
using PetRadar.SharedKernel.Events;

namespace Animals.Application.Commands.DeleteAnimalPost;

public sealed class DeleteAnimalPostCommandHandler : IRequestHandler<DeleteAnimalPostCommand>
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public DeleteAnimalPostCommandHandler(
        IAnimalRepository animalRepository,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _animalRepository = animalRepository;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(DeleteAnimalPostCommand request, CancellationToken cancellationToken)
    {
        var animalPost = await _animalRepository.GetByIdAsync(request.AnimalPostId, cancellationToken);

        if (animalPost is null)
            throw new AnimalPostNotFoundException(request.AnimalPostId);

        if (!string.Equals(animalPost.UserId, request.RequesterUserId, StringComparison.Ordinal))
            throw new AnimalDeleteForbiddenException(request.AnimalPostId, request.RequesterUserId);

        animalPost.Delete();

        await _animalRepository.DeleteAsync(request.AnimalPostId, cancellationToken);

        var domainEvents = animalPost.CollectDomainEvents();
        await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
