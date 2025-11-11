namespace EM.Domain.Common;

/// <summary>
/// Represents a base entity with a strongly-typed identifier.
/// </summary>
public abstract class Entity<T>
    where T : notnull
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public T ID { get; init; } = default!;
}
