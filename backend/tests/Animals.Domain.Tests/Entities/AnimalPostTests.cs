using Animals.Domain.Entities;
using Animals.Domain.Events;
using Animals.Domain.Exceptions;
using Animals.Domain.ValueObjects;
using PetRadar.SharedKernel.ValueObjects;

namespace Animals.Domain.Tests.Entities;

public sealed class AnimalPostTests
{
    private static GeoLocation ValidLocation => new(latitude: -23.5, longitude: -46.6);
    private const string ValidUserId = "user-123";
    private const string ValidDescription = "Animal perdido na rua";

    [Fact]
    public void Create_WithValidInputs_ReturnsPostWithExpectedFields()
    {
        var mediaIds = new List<string> { "m1", "m2" };

        var post = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, mediaIds);

        Assert.False(string.IsNullOrWhiteSpace(post.Id));
        Assert.Equal(ValidUserId, post.UserId);
        Assert.Equal(ValidDescription, post.Description);
        Assert.Equal(ValidLocation, post.Location);
        Assert.Equal(mediaIds, post.MediaIds);
        Assert.True((DateTime.UtcNow - post.CreatedAt).TotalSeconds < 5);
    }

    [Fact]
    public void Create_SetsStatusToLost()
    {
        var post = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, null);

        Assert.Equal(AnimalStatus.Lost(), post.Status);
    }

    [Fact]
    public void Create_WithMediaIds_SetsMediaIds()
    {
        var mediaIds = new List<string> { "m1", "m2" };

        var post = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, mediaIds);

        Assert.Equal(2, post.MediaIds.Count);
        Assert.Contains("m1", post.MediaIds);
        Assert.Contains("m2", post.MediaIds);
    }

    [Fact]
    public void Create_WithoutMediaIds_SetsEmptyMediaIds()
    {
        var post = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, null);

        Assert.Empty(post.MediaIds);
    }

    [Fact]
    public void Create_WithNullUserId_ThrowsInvalidAnimalUserIdException()
    {
        var act = () => AnimalPost.Create(null!, ValidDescription, ValidLocation, null);

        Assert.Throws<InvalidAnimalUserIdException>(act);
    }

    [Fact]
    public void Create_WithWhitespaceUserId_ThrowsInvalidAnimalUserIdException()
    {
        var act = () => AnimalPost.Create("   ", ValidDescription, ValidLocation, null);

        Assert.Throws<InvalidAnimalUserIdException>(act);
    }

    [Fact]
    public void Create_WithNullDescription_ThrowsInvalidAnimalDescriptionException()
    {
        var act = () => AnimalPost.Create(ValidUserId, null!, ValidLocation, null);

        Assert.Throws<InvalidAnimalDescriptionException>(act);
    }

    [Fact]
    public void Create_WithDescriptionShorterThanTenChars_ThrowsInvalidAnimalDescriptionException()
    {
        var act = () => AnimalPost.Create(ValidUserId, "curto", ValidLocation, null);

        Assert.Throws<InvalidAnimalDescriptionException>(act);
    }

    [Fact]
    public void Create_AccumulatesExactlyOneAnimalPostedEvent()
    {
        var post = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, null);

        var events = post.CollectDomainEvents();

        var domainEvent = Assert.Single(events);
        Assert.IsType<AnimalPostedEvent>(domainEvent);
    }

    [Fact]
    public void Create_AnimalPostedEventContainsCorrectPayload()
    {
        var post = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, null);

        var events = post.CollectDomainEvents();
        var postedEvent = Assert.IsType<AnimalPostedEvent>(Assert.Single(events));

        Assert.Equal(post.Id, postedEvent.AnimalPostId);
        Assert.Equal(ValidUserId, postedEvent.UserId);
        Assert.Equal(ValidLocation.Latitude, postedEvent.Latitude);
        Assert.Equal(ValidLocation.Longitude, postedEvent.Longitude);
    }

    [Fact]
    public void Create_TwoAnimalPosts_HaveDifferentIds()
    {
        var first = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, null);
        var second = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, null);

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void MarkAsFound_WhenLost_ChangesStatusToFound()
    {
        var post = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, null);
        post.CollectDomainEvents();

        post.MarkAsFound();

        Assert.Equal(AnimalStatus.Found(), post.Status);
    }

    [Fact]
    public void MarkAsFound_WhenLost_AccumulatesAnimalFoundEvent()
    {
        var post = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, null);
        post.CollectDomainEvents();

        post.MarkAsFound();
        var events = post.CollectDomainEvents();

        var foundEvent = Assert.IsType<AnimalFoundEvent>(Assert.Single(events));
        Assert.Equal(post.Id, foundEvent.AnimalPostId);
        Assert.Equal(ValidUserId, foundEvent.UserId);
    }

    [Fact]
    public void MarkAsFound_WhenAlreadyFound_ThrowsAnimalAlreadyFoundException()
    {
        var post = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, null);
        post.CollectDomainEvents();
        post.MarkAsFound();
        post.CollectDomainEvents();

        var act = () => post.MarkAsFound();

        Assert.Throws<AnimalAlreadyFoundException>(act);
    }

    [Fact]
    public void CollectDomainEvents_CalledTwice_ReturnsEmptyOnSecondCall()
    {
        var post = AnimalPost.Create(ValidUserId, ValidDescription, ValidLocation, null);
        post.CollectDomainEvents();

        var secondCollection = post.CollectDomainEvents();

        Assert.Empty(secondCollection);
    }
}
