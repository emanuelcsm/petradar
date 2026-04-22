using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Tests.ValueObjects;

public sealed class EmailTests
{
    [Fact]
    public void Constructor_WithNull_ThrowsInvalidEmailException()
    {
        var act = () => new Email(null!);

        Assert.Throws<InvalidEmailException>(act);
    }

    [Fact]
    public void Constructor_WithEmpty_ThrowsInvalidEmailException()
    {
        var act = () => new Email(string.Empty);

        Assert.Throws<InvalidEmailException>(act);
    }

    [Fact]
    public void Constructor_WithWhiteSpace_ThrowsInvalidEmailException()
    {
        var act = () => new Email("   ");

        Assert.Throws<InvalidEmailException>(act);
    }

    [Fact]
    public void Constructor_WithNoAtSign_ThrowsInvalidEmailException()
    {
        var act = () => new Email("invalidemail.com");

        Assert.Throws<InvalidEmailException>(act);
    }

    [Fact]
    public void Constructor_WithMultipleAtSigns_ThrowsInvalidEmailException()
    {
        var act = () => new Email("a@@b.com");

        Assert.Throws<InvalidEmailException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyLocalPart_ThrowsInvalidEmailException()
    {
        var act = () => new Email("@domain.com");

        Assert.Throws<InvalidEmailException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyDomain_ThrowsInvalidEmailException()
    {
        var act = () => new Email("user@");

        Assert.Throws<InvalidEmailException>(act);
    }

    [Fact]
    public void Constructor_WithExceededLength_ThrowsInvalidEmailException()
    {
        var longLocal = new string('a', 310);
        var act = () => new Email($"{longLocal}@domain.com");

        Assert.Throws<InvalidEmailException>(act);
    }

    [Fact]
    public void Constructor_WithValidEmail_SetsValue()
    {
        var email = new Email("user@example.com");

        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void Constructor_WithMixedCase_NormalizesToLowercase()
    {
        var email = new Email("User@Example.COM");

        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void Constructor_WithLeadingTrailingSpaces_Trims()
    {
        var email = new Email("  user@example.com  ");

        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void Equals_TwoEmailsWithSameValue_AreEqual()
    {
        var a = new Email("user@example.com");
        var b = new Email("user@example.com");

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_TwoEmailsWithDifferentValues_AreNotEqual()
    {
        var a = new Email("user@example.com");
        var b = new Email("other@example.com");

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void ToString_ReturnsNormalizedValue()
    {
        var email = new Email("User@Example.COM");

        Assert.Equal("user@example.com", email.ToString());
    }
}
