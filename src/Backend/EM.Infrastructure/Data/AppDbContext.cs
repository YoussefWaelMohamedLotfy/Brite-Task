using System.Reflection;

using EM.Domain.Entities;
using EM.Infrastructure.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace EM.Infrastructure.Data;

public sealed class AppDbContext(
    DbContextOptions options,
    UpdateAuditableEntitiesInterceptor updateAuditableEntitiesInterceptor)
    : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }

    public DbSet<Department> Departments { get; set; }

    public DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(updateAuditableEntitiesInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
