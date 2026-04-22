using Identity.Domain.Events;
using Identity.Domain.Exceptions;
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
        GeoLocation? alertLocation,
        DateTime createdAt) : base(id)
    {
        Email = email;
        Name = name;
        PasswordHash = passwordHash;
        AlertLocation = alertLocation;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// The only way to create a new <see cref="User"/>.
    /// Validates inputs, builds the Email value object, assigns an ID, and
    /// accumulates a <see cref="UserRegisteredEvent"/> to be dispatched after persistence.
    /// </summary>
    public static User Create(
        string email,
        string name,
        string passwordHash,
        GeoLocation? alertLocation = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidUserNameException("Name cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new InvalidPasswordHashException("Password hash cannot be null or empty.");

        var emailVo = new Email(email);
        var id = Guid.NewGuid().ToString();

        var user = new User(id, emailVo, name.Trim(), passwordHash, alertLocation, DateTime.UtcNow);

        user.AddDomainEvent(new UserRegisteredEvent(id, emailVo.Value, name.Trim()));

        return user;
    }

    /// <summary>
    /// Rehydrates an existing user from persistence without emitting new domain events.
    /// </summary>
    public static User Rehydrate(
        string id,
        string email,
        string name,
        string passwordHash,
        GeoLocation? alertLocation,
        DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidUserNameException("Name cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new InvalidPasswordHashException("Password hash cannot be null or empty.");

        var emailVo = new Email(email);
        return new User(id, emailVo, name.Trim(), passwordHash, alertLocation, createdAt);
    }
}
