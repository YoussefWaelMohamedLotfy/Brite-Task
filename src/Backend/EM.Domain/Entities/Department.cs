using EM.Domain.Common;

namespace EM.Domain.Entities;

/// <summary>
/// Represents a department within the organization.
/// </summary>
public sealed class Department : Entity<int>, IAuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the department.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the description of the department.
    /// </summary>
    public string? Description { get; set; }

    /// <inheritdoc/>
    public Guid CreatedBy { get; set; }

    /// <inheritdoc/>
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc/>
    public Guid? UpdatedBy { get; set; }

    /// <inheritdoc/>
    public DateTimeOffset? UpdatedAt { get; set; }
}
