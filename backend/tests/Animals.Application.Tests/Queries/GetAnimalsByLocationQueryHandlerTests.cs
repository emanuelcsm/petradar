using Animals.Application.Integration.Interfaces;
using Animals.Application.Interfaces;
using Animals.Application.Queries.GetAnimalsByLocation;
using Animals.Domain.Entities;
using Animals.Domain.Exceptions;
using NSubstitute;
using PetRadar.SharedKernel.Pagination;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Application.Tests.Queries;

public sealed class GetAnimalsByLocationQueryHandlerTests
{
    private readonly IAnimalRepository _animalRepository = Substitute.For<IAnimalRepository>();
    private readonly IKnownMediaRepository _knownMediaRepository = Substitute.For<IKnownMediaRepository>();

    private GetAnimalsByLocationQueryHandler CreateHandler() =>
        new(_animalRepository, _knownMediaRepository);

    private static GetAnimalsByLocationQuery ValidQuery(
        double radiusKm = 10,
        int pageSize = 10,
        int maxPageSize = 50,
        string? nextPageToken = null) =>
        new(Latitude: -23.5, Longitude: -46.6,
            RadiusKm: radiusKm, PageSize: pageSize, MaxPageSize: maxPageSize,
            NextPageToken: nextPageToken);

    private static AnimalPost CreatePost()
    {
        var location = new GeoLocation(latitude: -23.5, longitude: -46.6);
        var post = AnimalPost.Create("user-1", "Animal perdido na rua", location, null);
        post.CollectDomainEvents();
        return post;
    }

    [Fact]
    public async Task Handle_WithNegativeRadius_ThrowsInvalidNearbySearchRadiusException()
    {
        var query = ValidQuery(radiusKm: -1);

        await Assert.ThrowsAsync<InvalidNearbySearchRadiusException>(
            () => CreateHandler().Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithZeroRadius_ThrowsInvalidNearbySearchRadiusException()
    {
        var query = ValidQuery(radiusKm: 0);

        await Assert.ThrowsAsync<InvalidNearbySearchRadiusException>(
            () => CreateHandler().Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithPageSizeZero_ThrowsInvalidNearbySearchPageSizeException()
    {
        var query = ValidQuery(pageSize: 0);

        await Assert.ThrowsAsync<InvalidNearbySearchPageSizeException>(
            () => CreateHandler().Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithPageSizeExceedingMax_ThrowsInvalidNearbySearchPageSizeException()
    {
        var query = ValidQuery(pageSize: 100, maxPageSize: 50);

        await Assert.ThrowsAsync<InvalidNearbySearchPageSizeException>(
            () => CreateHandler().Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithLatitudeOutOfRange_ThrowsInvalidNearbySearchLocationException()
    {
        var query = new GetAnimalsByLocationQuery(
            Latitude: 999, Longitude: -46.6,
            RadiusKm: 10, PageSize: 10, MaxPageSize: 50, NextPageToken: null);

        await Assert.ThrowsAsync<InvalidNearbySearchLocationException>(
            () => CreateHandler().Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInvalidBase64PageToken_ThrowsInvalidNearbySearchPageTokenException()
    {
        var query = ValidQuery(nextPageToken: "@@@invalid@@@");

        await Assert.ThrowsAsync<InvalidNearbySearchPageTokenException>(
            () => CreateHandler().Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithValidRequest_ReturnsPagedResults()
    {
        var post = CreatePost();
        _animalRepository.GetNearbyAsync(
                Arg.Any<GeoLocation>(), Arg.Any<double>(),
                Arg.Any<DateTime?>(), Arg.Any<string?>(),
                Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new CursorSlice<AnimalPost>(new[] { post }, false));
        _knownMediaRepository.GetByIdsAsync(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<KnownMediaReadModel>());

        var result = await CreateHandler().Handle(ValidQuery(), CancellationToken.None);

        Assert.Single(result.Data);
        Assert.Equal(post.Id, result.Data[0].Id);
        Assert.False(result.HasNextPage);
        Assert.Null(result.NextPageToken);
    }

    [Fact]
    public async Task Handle_WhenRepositoryHasMoreItems_SetsHasNextPageTrue()
    {
        var post = CreatePost();
        _animalRepository.GetNearbyAsync(
                Arg.Any<GeoLocation>(), Arg.Any<double>(),
                Arg.Any<DateTime?>(), Arg.Any<string?>(),
                Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new CursorSlice<AnimalPost>(new[] { post }, true));
        _knownMediaRepository.GetByIdsAsync(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<KnownMediaReadModel>());

        var result = await CreateHandler().Handle(ValidQuery(), CancellationToken.None);

        Assert.True(result.HasNextPage);
        Assert.NotNull(result.NextPageToken);
    }

    [Fact]
    public async Task Handle_WithNoMedia_ReturnsResultsWithEmptyMediaList()
    {
        var post = CreatePost();
        _animalRepository.GetNearbyAsync(
                Arg.Any<GeoLocation>(), Arg.Any<double>(),
                Arg.Any<DateTime?>(), Arg.Any<string?>(),
                Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new CursorSlice<AnimalPost>(new[] { post }, false));
        _knownMediaRepository.GetByIdsAsync(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<KnownMediaReadModel>());

        var result = await CreateHandler().Handle(ValidQuery(), CancellationToken.None);

        Assert.Empty(result.Data[0].Media);
    }
}
