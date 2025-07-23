using EM.Domain.Common;

namespace EM.Domain.Entities;

public sealed class Role : Entity<int>, IAuditableEntity
{
    public string Name { get; set; } = default!;

    public List<string> Permissions { get; set; } = default!;

    public Guid CreatedBy { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public Guid UpdatedBy { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}
