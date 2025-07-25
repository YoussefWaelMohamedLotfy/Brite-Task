using EM.Domain.Entities;
using EM.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace EM.Infrastructure.UnitTests;

public abstract class InMemoryDbProvider
{
    protected readonly AppDbContext _context;

    protected InMemoryDbProvider()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new(options);
        Seed(_context);
    }

    private static void Seed(AppDbContext context)
    {
        // Create test roles
        var adminRole = new Role
        {
            Name = "Admin",
            Permissions = ["Create", "Read", "Update", "Delete"],
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        var userRole = new Role
        {
            Name = "User",
            Permissions = ["Read"],
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        context.Roles.AddRange(adminRole, userRole);

        // Create test departments
        var hrDepartment = new Department
        {
            Name = "HR",
            Description = "Human Resources",
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        var itDepartment = new Department
        {
            Name = "IT",
            Description = "Information Technology",
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        context.Departments.AddRange(hrDepartment, itDepartment);

        // Create test employees
        var employee1 = new Employee
        {
            Name = "Alice Smith",
            Email = "alice.smith@example.com",
            Phone = "123-456-7890",
            DateOfJoining = DateTimeOffset.UtcNow.AddYears(-2),
            IsActive = true,
            Department = hrDepartment,
            Role = adminRole,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        var employee2 = new Employee
        {
            Name = "Bob Johnson",
            Email = "bob.johnson@example.com",
            Phone = "987-654-3210",
            DateOfJoining = DateTimeOffset.UtcNow.AddYears(-1),
            IsActive = false,
            Department = itDepartment,
            Role = userRole,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        context.Employees.AddRange(employee1, employee2);
        context.SaveChanges();
    }
}
