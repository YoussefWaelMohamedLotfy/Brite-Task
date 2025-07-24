using EM.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EM.Infrastructure.Configurations;

/// <summary>
/// Configures the Employee entity for Entity Framework.
/// </summary>
internal sealed class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
{
    /// <summary>
    /// Configures the Employee entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasQueryFilter(x => x.IsActive);
        //builder.HasIndex(x => x.IsActive).HasFilter("Employees.IsActive = 1")
    }
}
