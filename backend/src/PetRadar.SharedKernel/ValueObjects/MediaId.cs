namespace PetRadar.SharedKernel.ValueObjects;

/// <summary>
/// Identifies a media file across all modules. Wraps the underlying string ID.
/// </summary>
public sealed record MediaId
{
    public string Value { get; }

    public MediaId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("MediaId cannot be null or empty.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}
