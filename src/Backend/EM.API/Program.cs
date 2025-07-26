using System.Security.Claims;
using System.Text.Json;

using EM.API.Auth;
using EM.Application;
using EM.Application.Features.Common.Behaviours;
using EM.Application.Features.Common.Exceptions;
using EM.Infrastructure.Data;
using EM.Infrastructure.Interceptors;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using MinimalApis.Discovery;

using Scalar.AspNetCore;


/// <summary>
/// Entry point for the EM.API application. Configures services, authentication, and endpoints.
/// </summary>
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.WebHost.ConfigureKestrel(x => x.AddServerHeader = false);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>(includeInternalTypes: true);

string oidcScheme = JwtBearerDefaults.AuthenticationScheme;

builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("HR-Policy", x => x.RequireAuthenticatedUser().AddRequirements(new RoleRequirement("HR")))
    .AddPolicy("Viewer-Policy", x => x.RequireAuthenticatedUser().AddRequirements(new RoleRequirement("Viewer")))
    .AddPolicy("Admin-Policy", x => x.RequireAuthenticatedUser().AddRequirements(new RoleRequirement("Admin")))
    .AddPolicy("HR-Viewer-Policy", x => x.RequireAuthenticatedUser().AddRequirements(new RoleRequirement("HR", "Viewer")))
    .AddPolicy("HR-Admin-Policy", x => x.RequireAuthenticatedUser().AddRequirements(new RoleRequirement("HR", "Admin")))
    .AddPolicy("Viewer-Admin-Policy", x => x.RequireAuthenticatedUser().AddRequirements(new RoleRequirement("Viewer", "Admin")))
    .AddPolicy("HR-Viewer-Admin-Policy", x => x.RequireAuthenticatedUser().AddRequirements(new RoleRequirement("HR", "Viewer", "Admin")));

builder.Services.AddAuthentication(oidcScheme)
                .AddKeycloakJwtBearer("keycloak", "tenant-1", oidcScheme, options =>
                {
                    options.Audience = "account";
                    options.SaveToken = true;
                    options.MapInboundClaims = true;
                    options.Events = new()
                    {
                        OnTokenValidated = ctx =>
                        {
                            ClaimsIdentity claimsIdentity = (ClaimsIdentity)ctx.Principal!.Identity!;
                            string? realm_access = claimsIdentity.FindFirst((claim) => claim.Type == "realm_access")?.Value;

                            using (JsonDocument doc = JsonDocument.Parse(realm_access))
                            {
                                if (doc.RootElement.TryGetProperty("roles", out JsonElement roleAccess) && roleAccess.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (JsonElement role in roleAccess.EnumerateArray())
                                    {
                                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!));
                                    }
                                }
                            }

                            return Task.CompletedTask;
                        }
                    };

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

    OpenApiSecurityScheme oauth2Scheme = new()
    {
        Name = "OAuth2",
        Scheme = "OAuth2",
        Type = SecuritySchemeType.OAuth2,
        Description = "OIDC authentication",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("http://localhost:8081/realms/tenant-1/protocol/openid-connect/auth"),
                TokenUrl = new Uri("http://localhost:8081/realms/tenant-1/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "openid" },
                    { "profile", "profile" }
                }
            }
        }
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
        document.Components.SecuritySchemes.Add("OAuth2", oauth2Scheme);
        return Task.CompletedTask;
    });

    x.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
        {
            operation.Security = [
                new() { [jwtScheme] = [] },
                new() { [oauth2Scheme] = [] },
            ];
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
    .AddAuthorizationCodeFlow("OAuth2", x =>
    {
        x.AuthorizationUrl = "http://localhost:8081/realms/tenant-1/protocol/openid-connect/auth";
        x.TokenUrl = "http://localhost:8081/realms/tenant-1/protocol/openid-connect/token";
        x.Pkce = Pkce.Sha256;
        x.RedirectUri = "https://localhost:7157/signin-oidc";
        x.ClientId = "backend-1";
        x.ClientSecret = "UfIMrte6w4gRCt2PYGL6ywPDtr1xR9cB";
        x.SelectedScopes = ["openid", "profile", "email", "offline_access"];
    })
    .AddPreferredSecuritySchemes("OAuth2"));

app.UseHttpsRedirection();
app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

app.MapApis();

await app.RunAsync();
