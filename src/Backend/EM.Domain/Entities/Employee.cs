using EM.Domain.Common;

namespace EM.Domain.Entities;

/// <summary>
/// Represents an employee entity with personal and employment details.
/// </summary>
public sealed class Employee : Entity<Guid>, IAuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the employee.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the email address of the employee.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Gets or sets the phone number of the employee.
    /// </summary>
    public string Phone { get; set; } = default!;

    /// <summary>
    /// Gets or sets the date the employee joined the organization.
    /// </summary>
    public DateTimeOffset DateOfJoining { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the employee is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the department to which the employee belongs.
    /// </summary>
    public Department Department { get; set; } = default!;

    /// <summary>
    /// Gets or sets the role assigned to the employee.
    /// </summary>
    public Role Role { get; set; } = default!;

    /// <inheritdoc/>
    public Guid CreatedBy { get; set; }

    /// <inheritdoc/>
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc/>
    public Guid? UpdatedBy { get; set; }

    /// <inheritdoc/>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Toggles the activation status of the employee.
    /// </summary>
    public void ToggleActivation() => IsActive = !IsActive;
}
