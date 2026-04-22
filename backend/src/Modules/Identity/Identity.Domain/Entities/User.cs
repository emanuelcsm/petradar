using Identity.Domain.ValueObjects;
using PetRadar.SharedKernel.Entities;
using PetRadar.SharedKernel.ValueObjects;

namespace Identity.Domain.Entities;

/// <summary>
/// Aggregate root for the Identity module.
/// All state changes must go through public methods on this class.
/// Construction is only permitted via the <c>Create</c> factory method (Fase 3).
/// </summary>
public sealed class User : AggregateRoot
{
    public Email Email { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public GeoLocation? AlertLocation { get; private set; }
    public DateTime CreatedAt { get; private init; }

    /// <summary>
    /// Parameterless constructor required by the MongoDB driver for deserialization.
    /// Must never be called by application code — use <c>Create</c>.
    /// </summary>
    private User() { }

    private User(
        string id,
        Email email,
        string name,
        string passwordHash,
        GeoLocation? alertLocation) : base(id)
    {
        Email = email;
        Name = name;
        PasswordHash = passwordHash;
        AlertLocation = alertLocation;
        CreatedAt = DateTime.UtcNow;
    }
}
