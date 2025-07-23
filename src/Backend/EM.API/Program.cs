using EM.Application;
using EM.Application.Features.Common.Behaviours;
using EM.Application.Features.Common.Exceptions;
using EM.Infrastructure.Data;
using EM.Infrastructure.Interceptors;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using MinimalApis.Discovery;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>(includeInternalTypes: true);

builder.Services.AddSingleton<UpdateAuditableEntitiesInterceptor>();

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.AddInterceptors(sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>());
    options.UseNpgsql(builder.Configuration.GetConnectionString("Employee-Management-Db"));
});
builder.EnrichNpgsqlDbContext<AppDbContext>();

builder.AddRedisOutputCache("garnet");

builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<IApplicationAssemblyMarker>()
    .AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>)));
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler(_ => { });
app.UseStatusCodePages();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(x => x.WithTheme(ScalarTheme.Default)
    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));

app.UseHttpsRedirection();
app.UseOutputCache();
app.MapApis();

await app.RunAsync();

