using Identity.Domain.Exceptions;

namespace Identity.Domain.ValueObjects;

/// <summary>
/// Email address value object. Immutable.
/// Normalizes to lowercase and trims whitespace on construction.
/// Validation: non-empty, contains exactly one '@', local-part and domain non-empty, max 320 chars.
/// </summary>
public sealed record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException("Email address cannot be empty.");

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > 320)
            throw new InvalidEmailException("Email address exceeds the maximum allowed length of 320 characters.");

        var atIndex = normalized.IndexOf('@');
        var lastAtIndex = normalized.LastIndexOf('@');

        if (atIndex < 0 || atIndex != lastAtIndex)
            throw new InvalidEmailException($"'{value}' is not a valid email address: must contain exactly one '@'.");

        var localPart = normalized[..atIndex];
        var domain = normalized[(atIndex + 1)..];

        if (localPart.Length == 0)
            throw new InvalidEmailException($"'{value}' is not a valid email address: local part cannot be empty.");

        if (domain.Length == 0)
            throw new InvalidEmailException($"'{value}' is not a valid email address: domain cannot be empty.");

        Value = normalized;
    }

    public override string ToString() => Value;
}
