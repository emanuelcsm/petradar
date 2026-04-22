using Animals.Domain.Exceptions;

namespace Animals.Domain.ValueObjects;

public sealed record AnimalStatus
{
    public string Value { get; }

    private AnimalStatus(string value)
    {
        if (value is not ("Lost" or "Found"))
            throw new InvalidAnimalStatusException(value);

        Value = value;
    }

    public static AnimalStatus Lost() => new("Lost");

    public static AnimalStatus Found() => new("Found");

    public static AnimalStatus From(string value) => new(value);

    public override string ToString() => Value;
}