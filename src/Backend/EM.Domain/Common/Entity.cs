namespace EM.Domain.Common;

public abstract class Entity<T> where T : notnull
{
    public T ID { get; init; } = default!;
}
