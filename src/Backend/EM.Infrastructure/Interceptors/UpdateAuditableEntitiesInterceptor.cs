using System.Security.Claims;

using EM.Domain.Common;
using EM.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EM.Infrastructure.Interceptors;

/// <summary>
/// Interceptor to update audit fields on auditable entities during save operations.
/// </summary>
public sealed class UpdateAuditableEntitiesInterceptor(ClaimsPrincipal claimsPrincipal) : SaveChangesInterceptor
{
    /// <summary>
    /// Called when saving changes asynchronously to update audit fields.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="result">The interception result.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask{InterceptionResult}"/> representing the asynchronous operation.</returns>
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
