using Animals.Application.Commands.MarkAsFound;
using Animals.Application.Interfaces;
using Animals.Domain.Entities;
using Animals.Domain.Exceptions;
using NSubstitute;
using PetRadar.SharedKernel.Events;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Application.Tests.Commands;

public sealed class MarkAsFoundCommandHandlerTests
{
    private readonly IAnimalRepository _animalRepository = Substitute.For<IAnimalRepository>();
    private readonly IDomainEventDispatcher _dispatcher = Substitute.For<IDomainEventDispatcher>();

    private const string OwnerId = "owner-123";

    private static AnimalPost CreateLostPost(string userId = OwnerId)
    {
        var location = new GeoLocation(latitude: -23.5, longitude: -46.6);
        return AnimalPost.Create(userId, "Animal perdido na rua", location, null);
    }

    private MarkAsFoundCommandHandler CreateHandler() =>
        new(_animalRepository, _dispatcher);

    [Fact]
    public async Task Handle_WithOwnerRequest_MarksAsFoundAndSavesAndDispatchesEvents()
    {
        var post = CreateLostPost();
        _animalRepository.GetByIdAsync(post.Id, Arg.Any<CancellationToken>()).Returns(post);

        var command = new MarkAsFoundCommand(post.Id, OwnerId);
        await CreateHandler().Handle(command, CancellationToken.None);

        await _animalRepository.Received(1).SaveAsync(
            Arg.Is<AnimalPost>(p => p.Id == post.Id),
            Arg.Any<CancellationToken>());
        await _dispatcher.Received(1).DispatchAsync(
            Arg.Any<IEnumerable<DomainEvent>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentPost_ThrowsAnimalPostNotFoundException()
    {
        _animalRepository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((AnimalPost?)null);

        var command = new MarkAsFoundCommand("post-inexistente", OwnerId);

        await Assert.ThrowsAsync<AnimalPostNotFoundException>(
            () => CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithWrongRequester_ThrowsAnimalOwnershipForbiddenException()
    {
        var post = CreateLostPost(userId: OwnerId);
        _animalRepository.GetByIdAsync(post.Id, Arg.Any<CancellationToken>()).Returns(post);

        var command = new MarkAsFoundCommand(post.Id, "outro-usuario");

        await Assert.ThrowsAsync<AnimalOwnershipForbiddenException>(
            () => CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPostAlreadyFound_ThrowsAnimalAlreadyFoundException()
    {
        var post = CreateLostPost();
        post.CollectDomainEvents();
        post.MarkAsFound();
        post.CollectDomainEvents();
        _animalRepository.GetByIdAsync(post.Id, Arg.Any<CancellationToken>()).Returns(post);

        var command = new MarkAsFoundCommand(post.Id, OwnerId);

        await Assert.ThrowsAsync<AnimalAlreadyFoundException>(
            () => CreateHandler().Handle(command, CancellationToken.None));
    }
}
