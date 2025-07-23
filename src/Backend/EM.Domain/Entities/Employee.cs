using EM.Domain.Common;

namespace EM.Domain.Entities;

public sealed class Employee : Entity<Guid>, IAuditableEntity
{
    public string Name { get; set; } = default!;
    
    public string Email { get; set; } = default!;
    
    public string Phone { get; set; } = default!;
    
    public DateTimeOffset DateOfJoining { get; set; }
    
    public bool IsActive { get; set; }

    public Guid CreatedBy { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public Guid UpdatedBy { get; init; }
    
    public DateTimeOffset UpdatedAt { get; init; }
}
