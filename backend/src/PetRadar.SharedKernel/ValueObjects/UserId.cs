namespace PetRadar.SharedKernel.ValueObjects;

/// <summary>
/// Identifies a user across all modules. Wraps the underlying string ID.
/// </summary>
public sealed record UserId
{
    public string Value { get; }

    public UserId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("UserId cannot be null or empty.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}
