namespace EM.Domain.Common;

public interface IAuditableEntity
{
    public Guid CreatedBy { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public Guid? UpdatedBy { get; init; }
    
    public DateTimeOffset? UpdatedAt { get; init; }
}
