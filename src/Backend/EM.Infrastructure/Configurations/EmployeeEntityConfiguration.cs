using EM.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EM.Infrastructure.Configurations;

internal sealed class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasQueryFilter(x => x.IsActive);
        //builder.HasIndex(x => x.IsActive).HasFilter("Employees.IsActive = 1")
    }
}
