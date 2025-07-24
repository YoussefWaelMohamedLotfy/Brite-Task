using EM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EM.Infrastructure.Data;

public static class AppDbContextInitializer
{
    public static async Task SeedInitialDataAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        // Seed Departments
        if (!await dbContext.Departments.AnyAsync(cancellationToken))
        {
            var departments = new List<Department>
                {
                    new()
                    {
                        Name = "Engineering",
                        Description = "Engineering Department",
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "Human Resources",
                        Description = "HR Department",
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "Finance",
                        Description = "Finance Department",
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    }
                };
            await dbContext.Departments.AddRangeAsync(departments, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        // Seed Roles
        if (!await dbContext.Roles.AnyAsync(cancellationToken))
        {
            var roles = new List<Role>
                {
                    new()
                    {
                        Name = "Admin",
                        Permissions = ["ManageUsers", "ViewReports", "EditSettings"],
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "HR",
                        Permissions = ["ViewEmployees", "EditEmployees"],
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "Viewer",
                        Permissions = ["ViewEmployees"],
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    }
                };
            await dbContext.Roles.AddRangeAsync(roles, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        // Seed Employees
        if (!await dbContext.Employees.AnyAsync(cancellationToken))
        {
            // Get departments and roles from the context
            var departments = dbContext.Departments.ToList();
            var roles = dbContext.Roles.ToList();
            var employees = new List<Employee>
                {
                    new()
                    {
                        Name = "Alice Admin",
                        Email = "alice@example.com",
                        Phone = "1234567890",
                        DateOfJoining = DateTimeOffset.UtcNow.AddYears(-2),
                        IsActive = true,
                        Department = departments.FirstOrDefault(d => d.Name == "Engineering"),
                        Role = roles.FirstOrDefault(r => r.Name == "Admin"),
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "Bob HR",
                        Email = "bob@example.com",
                        Phone = "2345678901",
                        DateOfJoining = DateTimeOffset.UtcNow.AddYears(-1),
                        IsActive = true,
                        Department = departments.FirstOrDefault(d => d.Name == "Human Resources"),
                        Role = roles.FirstOrDefault(r => r.Name == "HR"),
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "Admin",
                        Email = "admin@example.com",
                        Phone = "3456789012",
                        DateOfJoining = DateTimeOffset.UtcNow.AddMonths(-6),
                        IsActive = true,
                        Department = departments.FirstOrDefault(d => d.Name == "Finance"),
                        Role = roles.FirstOrDefault(r => r.Name == "Viewer"),
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    }
                };
            await dbContext.Employees.AddRangeAsync(employees, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public static void SeedInitialData(AppDbContext dbContext)
    {
        // Seed Departments
        if (!dbContext.Departments.Any())
        {
            var departments = new List<Department>
                {
                    new()
                    {
                        Name = "Engineering",
                        Description = "Engineering Department",
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "Human Resources",
                        Description = "HR Department",
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "Finance",
                        Description = "Finance Department",
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    }
                };
            dbContext.Departments.AddRange(departments);
            dbContext.SaveChanges();
        }

        // Seed Roles
        if (!dbContext.Roles.Any())
        {
            var roles = new List<Role>
                {
                    new()
                    {
                        Name = "Admin",
                        Permissions = ["ManageUsers", "ViewReports", "EditSettings"],
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "HR",
                        Permissions = ["ViewEmployees", "EditEmployees"],
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "Viewer",
                        Permissions = ["ViewEmployees"],
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    }
                };
            dbContext.Roles.AddRange(roles);
            dbContext.SaveChanges();
        }

        // Seed Employees
        if (!dbContext.Employees.Any())
        {
            // Get departments and roles from the context
            var departments = dbContext.Departments.ToList();
            var roles = dbContext.Roles.ToList();
            var employees = new List<Employee>
                {
                    new()
                    {
                        Name = "Alice Admin",
                        Email = "alice@example.com",
                        Phone = "1234567890",
                        DateOfJoining = DateTimeOffset.UtcNow.AddYears(-2),
                        IsActive = true,
                        Department = departments.FirstOrDefault(d => d.Name == "Engineering"),
                        Role = roles.FirstOrDefault(r => r.Name == "Admin"),
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "Bob HR",
                        Email = "bob@example.com",
                        Phone = "2345678901",
                        DateOfJoining = DateTimeOffset.UtcNow.AddYears(-1),
                        IsActive = true,
                        Department = departments.FirstOrDefault(d => d.Name == "Human Resources"),
                        Role = roles.FirstOrDefault(r => r.Name == "HR"),
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Name = "Admin",
                        Email = "admin@example.com",
                        Phone = "3456789012",
                        DateOfJoining = DateTimeOffset.UtcNow.AddMonths(-6),
                        IsActive = true,
                        Department = departments.FirstOrDefault(d => d.Name == "Finance"),
                        Role = roles.FirstOrDefault(r => r.Name == "Viewer"),
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.UtcNow
                    }
                };
            dbContext.Employees.AddRange(employees);
            dbContext.SaveChanges();
        }
    }
}
