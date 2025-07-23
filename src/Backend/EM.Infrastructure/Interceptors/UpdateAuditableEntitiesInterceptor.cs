using System.Security.Claims;

using EM.Domain.Common;
using EM.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EM.Infrastructure.Interceptors;

public sealed class UpdateAuditableEntitiesInterceptor(ClaimsPrincipal claimsPrincipal) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var dbcontext = eventData.Context;

        if (dbcontext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        IEnumerable<EntityEntry<IAuditableEntity>> entries = dbcontext.ChangeTracker.Entries<IAuditableEntity>();

        foreach (EntityEntry<IAuditableEntity> entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                entityEntry.Entity.CreatedBy = claimsPrincipal.GetUserId();
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                entityEntry.Entity.UpdatedBy = claimsPrincipal.GetUserId();
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
