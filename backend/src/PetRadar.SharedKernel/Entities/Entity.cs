namespace PetRadar.SharedKernel.Entities;

public abstract class Entity
{
    public string Id { get; protected init; } = default!;

    protected Entity() { }

    protected Entity(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Entity Id cannot be null or empty.", nameof(id));

        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Id == other.Id;
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity? left, Entity? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity? left, Entity? right)
        => !(left == right);
}
