using Animals.Application.Integration.Interfaces;
using Animals.Application.Interfaces;
using Animals.Domain.Exceptions;
using Animals.Domain.Entities;
using MediatR;
using PetRadar.SharedKernel.Events;

namespace Animals.Application.Commands.CreateAnimalPost;

public sealed class CreateAnimalPostCommandHandler : IRequestHandler<CreateAnimalPostCommand>
{
    private readonly IKnownMediaRepository _knownMediaRepository;
    private readonly IAnimalRepository _animalRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public CreateAnimalPostCommandHandler(
        IKnownMediaRepository knownMediaRepository,
        IAnimalRepository animalRepository,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _knownMediaRepository = knownMediaRepository;
        _animalRepository = animalRepository;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(CreateAnimalPostCommand request, CancellationToken cancellationToken)
    {
        if (request.MediaIds is not null)
        {
            foreach (var mediaId in request.MediaIds.Distinct())
            {
                var mediaExists = await _knownMediaRepository.ExistsByIdAsync(mediaId, cancellationToken);

                if (!mediaExists)
                    throw new AnimalMediaNotFoundException(mediaId);
            }
        }

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
