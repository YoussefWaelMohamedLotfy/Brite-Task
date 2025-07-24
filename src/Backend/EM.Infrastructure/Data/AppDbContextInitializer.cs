using EM.Domain.Entities;

namespace EM.Infrastructure.Data;

public static class AppDbContextInitializer
{
    public static async Task SeedInitialDataAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        // Seed Departments
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

        // Seed Roles
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

        // Seed Employees
        var employees = new List<Employee>
            {
                new()
                {
                    Name = "Alice Admin",
                    Email = "alice@example.com",
                    Phone = "1234567890",
                    DateOfJoining = DateTimeOffset.UtcNow.AddYears(-2),
                    IsActive = true,
                    Department = departments[0], // Engineering
                    Role = roles[0], // Admin
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
                    Department = departments[1], // Human Resources
                    Role = roles[1], // HR
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
                    Department = departments[2], // Finance
                    Role = roles[2], // Viewer
                    CreatedBy = Guid.Empty,
                    CreatedAt = DateTimeOffset.UtcNow
                }
            };
        await dbContext.Employees.AddRangeAsync(employees, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public static void SeedInitialData(AppDbContext dbContext)
    {
        // Seed Departments
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

        // Seed Roles
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

        // Seed Employees
        var employees = new List<Employee>
            {
                new()
                {
                    Name = "Alice Admin",
                    Email = "alice@example.com",
                    Phone = "1234567890",
                    DateOfJoining = DateTimeOffset.UtcNow.AddYears(-2),
                    IsActive = true,
                    Department = departments[0], // Engineering
                    Role = roles[0], // Admin
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
                    Department = departments[1], // Human Resources
                    Role = roles[1], // HR
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
                    Department = departments[2], // Finance
                    Role = roles[2], // Viewer
                    CreatedBy = Guid.Empty,
                    CreatedAt = DateTimeOffset.UtcNow
                }
            };
        dbContext.Employees.AddRange(employees);
        dbContext.SaveChanges();
    }
}
