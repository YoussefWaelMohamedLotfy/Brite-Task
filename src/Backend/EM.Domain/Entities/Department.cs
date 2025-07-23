using EM.Domain.Common;

namespace EM.Domain.Entities;

public sealed class Department : Entity<int>, IAuditableEntity
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public Guid CreatedBy { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public Guid UpdatedBy { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}
