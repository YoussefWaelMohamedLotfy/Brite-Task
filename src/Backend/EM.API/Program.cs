using EM.Infrastructure.Data;
using EM.Infrastructure.Interceptors;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<UpdateAuditableEntitiesInterceptor>();

builder.AddNpgsqlDbContext<AppDbContext>("Employee-Management-Db");

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(x => x.WithTheme(ScalarTheme.Default)
    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));

app.UseHttpsRedirection();

await app.RunAsync();

