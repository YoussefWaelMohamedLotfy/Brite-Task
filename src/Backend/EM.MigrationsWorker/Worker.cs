using System.Diagnostics;

using EM.Domain.Entities;
using EM.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace EM.MigrationsWorker;

public sealed class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await RunMigrationAsync(dbContext, stoppingToken);

            if ((await dbContext.Database.GetPendingMigrationsAsync(stoppingToken)).Any())
            {
                await SeedDataAsync(dbContext, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

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
                    Email = "alice.admin@example.com",
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
                    Email = "bob.hr@example.com",
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
                    Name = "Charlie Viewer",
                    Email = "charlie.viewer@example.com",
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

            await transaction.CommitAsync(cancellationToken);
        });
    }
}