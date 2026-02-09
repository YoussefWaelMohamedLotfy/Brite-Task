using System.Reflection;
using EM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EM.Infrastructure.Data;

/// <summary>
/// Represents the Entity Framework database context for the application.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the Employees table.
    /// </summary>
    public DbSet<Employee> Employees { get; set; }

    /// <summary>
    /// Gets or sets the Departments table.
    /// </summary>
    public DbSet<Department> Departments { get; set; }

    /// <summary>
    /// Gets or sets the Roles table.
    /// </summary>
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Configures the model for the context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public bool BypassInterceptors { get; private set; } = false;

    public IDisposable SuppressInterceptors()
    {
        BypassInterceptors = true;
        return new InterceptorScope(this);
    }

    private sealed class InterceptorScope(AppDbContext context) : IDisposable
    {
        public void Dispose()
        {
            context.BypassInterceptors = false;
        }
    }
}

/// <summary>
/// Factory for creating <see cref="AppDbContext"/> at design time.
/// </summary>
public sealed class BloggingContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Creates a new instance of <see cref="AppDbContext"/> for design-time operations.
    /// </summary>
    /// <param name="args">Arguments passed by the design-time tool.</param>
    /// <returns>A new <see cref="AppDbContext"/> instance.</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Username=pg;Password=EnterPassword;Database=Employee-Management-Db"
        );
        return new AppDbContext(optionsBuilder.Options);
    }
}
