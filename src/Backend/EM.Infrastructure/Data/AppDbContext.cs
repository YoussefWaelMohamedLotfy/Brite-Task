using EM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EM.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }

    public DbSet<Department> Departments { get; set; }
}
