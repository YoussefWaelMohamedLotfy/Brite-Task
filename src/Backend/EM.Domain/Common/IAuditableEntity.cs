namespace EM.Domain.Common;

public interface IAuditableEntity
{
    public Guid CreatedBy { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public Guid? UpdatedBy { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}
