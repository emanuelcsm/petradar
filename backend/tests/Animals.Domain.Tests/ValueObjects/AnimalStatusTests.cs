using Animals.Domain.Exceptions;
using Animals.Domain.ValueObjects;

namespace Animals.Domain.Tests.ValueObjects;

public sealed class AnimalStatusTests
{
    [Fact]
    public void Lost_ReturnsStatusWithValueLost()
    {
        var status = AnimalStatus.Lost();

        Assert.Equal("Lost", status.Value);
    }

    [Fact]
    public void Found_ReturnsStatusWithValueFound()
    {
        var status = AnimalStatus.Found();

        Assert.Equal("Found", status.Value);
    }

    [Fact]
    public void From_WithLostString_ReturnsLostStatus()
    {
        var status = AnimalStatus.From("Lost");

        Assert.Equal(AnimalStatus.Lost(), status);
    }

    [Fact]
    public void From_WithFoundString_ReturnsFoundStatus()
    {
        var status = AnimalStatus.From("Found");

        Assert.Equal(AnimalStatus.Found(), status);
    }

    [Fact]
    public void From_WithInvalidString_ThrowsInvalidAnimalStatusException()
    {
        var act = () => AnimalStatus.From("Invalid");

        Assert.Throws<InvalidAnimalStatusException>(act);
    }

    [Fact]
    public void From_WithNull_ThrowsInvalidAnimalStatusException()
    {
        var act = () => AnimalStatus.From(null!);

        Assert.Throws<InvalidAnimalStatusException>(act);
    }

    [Fact]
    public void Equals_TwoLostStatuses_AreEqual()
    {
        var a = AnimalStatus.Lost();
        var b = AnimalStatus.Lost();

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_LostAndFound_AreNotEqual()
    {
        var lost = AnimalStatus.Lost();
        var found = AnimalStatus.Found();

        Assert.NotEqual(lost, found);
    }
}
