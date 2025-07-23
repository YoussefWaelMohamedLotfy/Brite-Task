using EM.Application;
using EM.Infrastructure.Data;
using EM.Infrastructure.Interceptors;

using MinimalApis.Discovery;
using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<UpdateAuditableEntitiesInterceptor>();


builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.AddInterceptors(sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>());
    options.UseNpgsql(builder.Configuration.GetConnectionString("Employee-Management-Db"));
});
builder.EnrichNpgsqlDbContext<AppDbContext>();

builder.AddRedisOutputCache("garnet");

builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<IApplicationAssemblyMarker>());

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(x => x.WithTheme(ScalarTheme.Default)
    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));

app.UseHttpsRedirection();
app.UseOutputCache();
app.MapApis();

await app.RunAsync();

