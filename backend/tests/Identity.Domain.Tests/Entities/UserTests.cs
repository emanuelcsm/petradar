using Identity.Domain.Entities;
using Identity.Domain.Events;
using Identity.Domain.Exceptions;
using PetRadar.SharedKernel.ValueObjects;

namespace Identity.Domain.Tests.Entities;

public sealed class UserTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsUserWithExpectedFields()
    {
        var user = User.Create("user@example.com", "John Doe", "hashed_password");

        Assert.Equal("user@example.com", user.Email.Value);
        Assert.Equal("John Doe", user.Name);
        Assert.Equal("hashed_password", user.PasswordHash);
        Assert.Null(user.AlertLocation);
        Assert.False(string.IsNullOrWhiteSpace(user.Id));
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_WithAlertLocation_SetsAlertLocation()
    {
        var location = new GeoLocation(-23.5, -46.6);

        var user = User.Create("user@example.com", "John Doe", "hashed_password", location);

        Assert.NotNull(user.AlertLocation);
        Assert.Equal(-23.5, user.AlertLocation.Latitude);
        Assert.Equal(-46.6, user.AlertLocation.Longitude);
    }

    [Fact]
    public void Create_WithInvalidEmail_PropagatesInvalidEmailException()
    {
        var act = () => User.Create("not-an-email", "John Doe", "hashed_password");

        Assert.Throws<InvalidEmailException>(act);
    }

    [Fact]
    public void Create_WithNullName_ThrowsInvalidUserNameException()
    {
        var act = () => User.Create("user@example.com", null!, "hashed_password");

        Assert.Throws<InvalidUserNameException>(act);
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsInvalidUserNameException()
    {
        var act = () => User.Create("user@example.com", "   ", "hashed_password");

        Assert.Throws<InvalidUserNameException>(act);
    }

    [Fact]
    public void Create_WithNullPasswordHash_ThrowsInvalidPasswordHashException()
    {
        var act = () => User.Create("user@example.com", "John Doe", null!);

        Assert.Throws<InvalidPasswordHashException>(act);
    }

    [Fact]
    public void Create_WithEmptyPasswordHash_ThrowsInvalidPasswordHashException()
    {
        var act = () => User.Create("user@example.com", "John Doe", "   ");

        Assert.Throws<InvalidPasswordHashException>(act);
    }

    [Fact]
    public void Create_AccumulatesExactlyOneUserRegisteredEvent()
    {
        var user = User.Create("user@example.com", "John Doe", "hashed_password");

        var events = user.CollectDomainEvents();

        Assert.Single(events);
        Assert.IsType<UserRegisteredEvent>(events[0]);
    }

    [Fact]
    public void Create_UserRegisteredEventContainsCorrectPayload()
    {
        var user = User.Create("user@example.com", "John Doe", "hashed_password");

        var events = user.CollectDomainEvents();
        var evt = Assert.IsType<UserRegisteredEvent>(events[0]);

        Assert.Equal(user.Id, evt.UserId);
        Assert.Equal("user@example.com", evt.Email);
        Assert.Equal("John Doe", evt.Name);
    }

    [Fact]
    public void Create_TwoUsers_HaveDifferentIds()
    {
        var user1 = User.Create("a@example.com", "Alice", "hash1");
        var user2 = User.Create("b@example.com", "Bob", "hash2");

        Assert.NotEqual(user1.Id, user2.Id);
    }

    [Fact]
    public void CollectDomainEvents_CalledTwice_ReturnsEmptyOnSecondCall()
    {
        var user = User.Create("user@example.com", "John Doe", "hashed_password");

        user.CollectDomainEvents();
        var second = user.CollectDomainEvents();

        Assert.Empty(second);
    }
}
