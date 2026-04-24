using Animals.Application.Commands.CreateAnimalPost;
using Animals.Application.Integration.Interfaces;
using Animals.Application.Interfaces;
using Animals.Domain.Entities;
using Animals.Domain.Exceptions;
using NSubstitute;
using PetRadar.SharedKernel.Events;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Application.Tests.Commands;

public sealed class CreateAnimalPostCommandHandlerTests
{
    private readonly IKnownMediaRepository _knownMediaRepository = Substitute.For<IKnownMediaRepository>();
    private readonly IAnimalRepository _animalRepository = Substitute.For<IAnimalRepository>();
    private readonly IDomainEventDispatcher _dispatcher = Substitute.For<IDomainEventDispatcher>();

    private static GeoLocation ValidLocation => new(latitude: -23.5, longitude: -46.6);
    private const string ValidUserId = "user-123";
    private const string ValidDescription = "Animal perdido na rua";

    private CreateAnimalPostCommandHandler CreateHandler() =>
        new(_knownMediaRepository, _animalRepository, _dispatcher);

    [Fact]
    public async Task Handle_WithValidDataAndNoMedia_SavesPostAndDispatchesEvents()
    {
        var command = new CreateAnimalPostCommand(ValidUserId, ValidDescription, ValidLocation, null);

        await CreateHandler().Handle(command, CancellationToken.None);

        await _animalRepository.Received(1).SaveAsync(Arg.Any<AnimalPost>(), Arg.Any<CancellationToken>());
        await _dispatcher.Received(1).DispatchAsync(
            Arg.Any<IEnumerable<DomainEvent>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidMediaIds_VerifiesEachMediaExists()
    {
        var mediaIds = new List<string> { "media-1", "media-2" };
        _knownMediaRepository.ExistsByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var command = new CreateAnimalPostCommand(ValidUserId, ValidDescription, ValidLocation, mediaIds);
        await CreateHandler().Handle(command, CancellationToken.None);

        await _knownMediaRepository.Received(1).ExistsByIdAsync("media-1", Arg.Any<CancellationToken>());
        await _knownMediaRepository.Received(1).ExistsByIdAsync("media-2", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnknownMediaId_ThrowsAnimalMediaNotFoundException()
    {
        _knownMediaRepository.ExistsByIdAsync("media-inexistente", Arg.Any<CancellationToken>())
            .Returns(false);

        var command = new CreateAnimalPostCommand(
            ValidUserId, ValidDescription, ValidLocation,
            new List<string> { "media-inexistente" });

        await Assert.ThrowsAsync<AnimalMediaNotFoundException>(
            () => CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CallsSaveAsyncOnRepository()
    {
        var command = new CreateAnimalPostCommand(ValidUserId, ValidDescription, ValidLocation, null);

        await CreateHandler().Handle(command, CancellationToken.None);

        await _animalRepository.Received(1).SaveAsync(
            Arg.Is<AnimalPost>(p =>
                p.UserId == ValidUserId &&
                p.Description == ValidDescription),
            Arg.Any<CancellationToken>());
    }
}
