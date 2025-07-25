using EM.Domain.Entities;
using EM.Infrastructure.Data;
using Bogus;

using Microsoft.EntityFrameworkCore;

namespace EM.Application.UnitTests;

public sealed class InMemoryDbProvider : IAsyncLifetime
{
    public AppDbContext DbContext { get; }

    public InMemoryDbProvider()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        DbContext = new(options);
    }

    private static async Task SeedAsync(AppDbContext context)
    {
        // Generate 5 roles
        var roleFaker = new Faker<Role>()
            .RuleFor(r => r.Name, f => f.Person.FullName)
            .RuleFor(r => r.Permissions, f => f.Make(f.Random.Int(1, 5), () => f.PickRandom(new[] { "Create", "Read", "Update", "Delete", "Approve" })))
            .RuleFor(r => r.CreatedBy, f => f.Random.Guid())
            .RuleFor(r => r.CreatedAt, f => f.Date.PastOffset(3));
        var roles = roleFaker.Generate(5);
        await context.Roles.AddRangeAsync(roles);

        // Generate 5 departments
        var departmentFaker = new Faker<Department>()
            .RuleFor(d => d.Name, f => f.Commerce.Department())
            .RuleFor(d => d.Description, f => f.Lorem.Sentence())
            .RuleFor(d => d.CreatedBy, f => f.Random.Guid())
            .RuleFor(d => d.CreatedAt, f => f.Date.PastOffset(3));
        var departments = departmentFaker.Generate(5);
        await context.Departments.AddRangeAsync(departments);

        // Generate 50 employees
        var employeeFaker = new Faker<Employee>()
            .RuleFor(e => e.Name, f => f.Name.FullName())
            .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.Name))
            .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(e => e.DateOfJoining, f => f.Date.PastOffset(5))
            .RuleFor(e => e.IsActive, f => f.Random.Bool())
            .RuleFor(e => e.Department, f => f.PickRandom(departments))
            .RuleFor(e => e.Role, f => f.PickRandom(roles))
            .RuleFor(e => e.CreatedBy, f => f.Random.Guid())
            .RuleFor(e => e.CreatedAt, f => f.Date.PastOffset(3));
        var employees = employeeFaker.Generate(50);
        await context.Employees.AddRangeAsync(employees);
        await context.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        GC.SuppressFinalize(this);
    }

    public async ValueTask InitializeAsync()
    {
        await DbContext.Database.EnsureCreatedAsync();
        await SeedAsync(DbContext);
    }
}
