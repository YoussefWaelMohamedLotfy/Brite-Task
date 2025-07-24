using EM.Domain.Common;

namespace EM.Domain.Entities;

/// <summary>
/// Represents a role assigned to employees, including permissions.
/// </summary>
public sealed class Role : Entity<int>, IAuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the list of permissions associated with the role.
    /// </summary>
    public List<string> Permissions { get; set; } = default!;

    /// <inheritdoc/>
    public Guid CreatedBy { get; set; }

    /// <inheritdoc/>
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc/>
    public Guid? UpdatedBy { get; set; }

    /// <inheritdoc/>
    public DateTimeOffset? UpdatedAt { get; set; }
}
