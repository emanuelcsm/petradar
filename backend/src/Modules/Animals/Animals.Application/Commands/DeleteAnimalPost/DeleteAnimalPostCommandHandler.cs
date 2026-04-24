using Animals.Application.Interfaces;
using Animals.Domain.Exceptions;
using MediatR;

namespace Animals.Application.Commands.DeleteAnimalPost;

public sealed class DeleteAnimalPostCommandHandler : IRequestHandler<DeleteAnimalPostCommand>
{
    private readonly IAnimalRepository _animalRepository;

    public DeleteAnimalPostCommandHandler(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    public async Task Handle(DeleteAnimalPostCommand request, CancellationToken cancellationToken)
    {
        var animalPost = await _animalRepository.GetByIdAsync(request.AnimalPostId, cancellationToken);

        if (animalPost is null)
            throw new AnimalPostNotFoundException(request.AnimalPostId);

        if (!string.Equals(animalPost.UserId, request.RequesterUserId, StringComparison.Ordinal))
            throw new AnimalDeleteForbiddenException(request.AnimalPostId, request.RequesterUserId);

        await _animalRepository.DeleteAsync(request.AnimalPostId, cancellationToken);
    }
}
