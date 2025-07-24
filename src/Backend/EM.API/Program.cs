using System.Security.Claims;

using EM.Application;
using EM.Application.Features.Common.Behaviours;
using EM.Application.Features.Common.Exceptions;
using EM.Infrastructure.Data;
using EM.Infrastructure.Interceptors;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using MinimalApis.Discovery;

using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>(includeInternalTypes: true);

string oidcScheme = OpenIdConnectDefaults.AuthenticationScheme;

builder.Services.AddAuthorizationBuilder();

builder.Services.AddAuthentication(oidcScheme)
                .AddKeycloakJwtBearer("keycloak", "tenant-1", oidcScheme, options =>
                {
                    options.Audience = "account";
                    options.SaveToken = true;
                    options.MapInboundClaims = true;

                    // For development only - disable HTTPS metadata validation
                    // In production, use explicit Authority configuration instead
                    options.RequireHttpsMetadata = builder.Environment.IsProduction();
                });

builder.Services.AddSingleton<UpdateAuditableEntitiesInterceptor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient(s =>
{
    IHttpContextAccessor contextAccessor = s.GetRequiredService<IHttpContextAccessor>();
    ClaimsPrincipal? user = contextAccessor?.HttpContext?.User;
    return user ?? throw new NullReferenceException("User not resolved");
});

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.AddInterceptors(sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>());
    options.UseNpgsql(builder.Configuration.GetConnectionString("Employee-Management-Db"))
    .UseAsyncSeeding(async (context, _, ct) => await AppDbContextInitializer.SeedInitialDataAsync((AppDbContext)context, ct))
    .UseSeeding((context, _) => AppDbContextInitializer.SeedInitialData((AppDbContext)context));
});
builder.EnrichNpgsqlDbContext<AppDbContext>();

builder.AddRedisOutputCache("garnet");

builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<IApplicationAssemblyMarker>()
    .AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>)));

builder.Services.AddOpenApi(x =>
{
    OpenApiSecurityScheme jwtScheme = new()
    {
        Type = SecuritySchemeType.Http,
        Name = JwtBearerDefaults.AuthenticationScheme,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Reference = new()
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        },
    };

    x.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Contact = new()
        {
            Name = "Brite Support",
            Email = "info@gobrite.ai"
        };


        document.Components ??= new();
        document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme, jwtScheme);

        return Task.CompletedTask;
    });

    x.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
        {
            operation.Security = [new() { [jwtScheme] = [] }];
        }

        return Task.CompletedTask;
    });
});

WebApplication app = builder.Build();

app.UseExceptionHandler(_ => { });
app.UseStatusCodePages();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(x => x.WithTheme(ScalarTheme.Default)
    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
    .AddPreferredSecuritySchemes("Bearer"));

app.UseHttpsRedirection();
app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

app.MapApis();

await app.RunAsync();
