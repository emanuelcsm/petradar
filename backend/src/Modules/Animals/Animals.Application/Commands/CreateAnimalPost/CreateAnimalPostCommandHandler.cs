using Animals.Application.Interfaces;
using Animals.Domain.Entities;
using MediatR;
using PetRadar.SharedKernel.Events;

namespace Animals.Application.Commands.CreateAnimalPost;

public sealed class CreateAnimalPostCommandHandler : IRequestHandler<CreateAnimalPostCommand>
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public CreateAnimalPostCommandHandler(
        IAnimalRepository animalRepository,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _animalRepository = animalRepository;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(CreateAnimalPostCommand request, CancellationToken cancellationToken)
    {
        var animalPost = AnimalPost.Create(
            request.UserId,
            request.Description,
            request.Location,
            request.MediaIds);

        await _animalRepository.SaveAsync(animalPost, cancellationToken);

        var domainEvents = animalPost.CollectDomainEvents();
        await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
