using System.Diagnostics;

using EM.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace EM.MigrationsWorker;

/// <summary>
/// Background service responsible for applying database migrations and seeding data at application startup.
/// </summary>
/// <param name="serviceProvider">The application's service provider.</param>
/// <param name="hostApplicationLifetime">The application lifetime manager.</param>
public sealed class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    /// <summary>
    /// The name of the activity source for tracing.
    /// </summary>
    public const string ActivitySourceName = "Migrations";
    /// <summary>
    /// The activity source used for tracing migration operations.
    /// </summary>
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    /// <summary>
    /// Executes the background service to run migrations and seed data.
    /// </summary>
    /// <param name="stoppingToken">A cancellation token.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await RunMigrationAsync(dbContext, stoppingToken);
            await SeedDataAsync(dbContext, stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    /// <summary>
    /// Runs the database migrations using the execution strategy.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    private static async Task RunMigrationAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    /// <summary>
    /// Seeds the database with initial data using the execution strategy.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    private static async Task SeedDataAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await AppDbContextInitializer.SeedInitialDataAsync(dbContext, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });
    }
}