using EM.Domain.Common;

namespace EM.Domain.Entities;

public sealed class Role : Entity<int>, IAuditableEntity
{
    public string Name { get; set; } = default!;

    public List<string> Permissions { get; set; } = default!;

    public Guid CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
