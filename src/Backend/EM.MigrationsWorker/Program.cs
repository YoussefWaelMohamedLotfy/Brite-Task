using EM.Infrastructure.Data;
using EM.MigrationsWorker;


/// <summary>
/// Entry point for the Migrations Worker application.
/// Configures services and runs the host.
/// </summary>
var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<AppDbContext>("Employee-Management-Db");

var host = builder.Build();
await host.RunAsync();
