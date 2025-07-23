using EM.Domain.Common;

namespace EM.Domain.Entities;

public sealed class Employee : Entity<Guid>, IAuditableEntity
{
    public string Name { get; set; } = default!;
    
    public string Email { get; set; } = default!;
    
    public string Phone { get; set; } = default!;
    
    public DateTimeOffset DateOfJoining { get; set; }
    
    public bool IsActive { get; set; }

    public Department Department { get; set; } = default!;

    public Role Role { get; set; } = default!;

    public Guid CreatedBy { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public Guid? UpdatedBy { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}
