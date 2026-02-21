using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using EM.Application;
using EM.Application.Features.Common.Behaviours;
using EM.Infrastructure.Data;
using EM.Infrastructure.Interceptors;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddKeycloakJwtBearer(
        "keycloak",
        "tenant-1",
        JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            options.Audience = "account";
            options.SaveToken = true;
            options.MapInboundClaims = true;
            options.RequireHttpsMetadata = builder.Environment.IsProduction();

            // Configure to validate tokens from our OAuth server
            options.TokenValidationParameters = new TokenValidationParameters
            {
                //ValidateIssuer = true,
                //ValidateAudience = true,
                //ValidateLifetime = true,
                //ValidateIssuerSigningKey = true,
                //ValidAudience = serverUrl, // Validate that the audience matches the resource metadata as suggested in RFC 8707
                //ValidIssuer = options.Authority,
                NameClaimType = "name",
                RoleClaimType = "roles",
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    ClaimsIdentity claimsIdentity = (ClaimsIdentity)context.Principal!.Identity!;
                    string? realm_access = claimsIdentity
                        .FindFirst((claim) => claim.Type == "realm_access")
                        ?.Value;

                    using (JsonDocument doc = JsonDocument.Parse(realm_access))
                    {
                        if (
                            doc.RootElement.TryGetProperty("roles", out JsonElement roleAccess)
                            && roleAccess.ValueKind == JsonValueKind.Array
                        )
                        {
                            foreach (JsonElement role in roleAccess.EnumerateArray())
                            {
                                claimsIdentity.AddClaim(
                                    new Claim(ClaimTypes.Role, role.GetString()!)
                                );
                            }
                        }
                    }

                    var name = context.Principal?.Identity?.Name ?? "unknown";
                    var email =
                        context.Principal?.FindFirstValue("preferred_username") ?? "unknown";
                    Console.WriteLine($"Token validated for: {name} ({email})");
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine($"Challenging client to authenticate with Entra ID");
                    return Task.CompletedTask;
                },
            };
        }
    )
    .AddMcp(options =>
    {
        options.ResourceMetadata = new()
        {
            Resource = "https://localhost:7077/",
            ResourceDocumentation = "https://docs.example.com/api/weather",
            AuthorizationServers = { "http://localhost:8081/realms/tenant-1" },
            ScopesSupported = ["openid", "profile"],
        };
    });

builder.Services.AddAuthorizationBuilder();
builder.Services.AddSingleton<UpdateAuditableEntitiesInterceptor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient(s =>
{
    IHttpContextAccessor contextAccessor = s.GetRequiredService<IHttpContextAccessor>();
    ClaimsPrincipal? user = contextAccessor?.HttpContext?.User;
    return user ?? throw new NullReferenceException("User not resolved");
});

builder.Services.AddDbContextPool<AppDbContext>(
    (sp, options) =>
    {
        options.AddInterceptors(sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>());
        options
            .UseNpgsql(builder.Configuration.GetConnectionString("Employee-Management-Db"))
            .UseAsyncSeeding(
                async (context, _, ct) =>
                    await AppDbContextInitializer.SeedInitialDataAsync((AppDbContext)context, ct)
            )
            .UseSeeding(
                (context, _) => AppDbContextInitializer.SeedInitialData((AppDbContext)context)
            );
    }
);
builder.EnrichNpgsqlDbContext<AppDbContext>();

builder.Services.AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>(
    includeInternalTypes: true
);

builder.Services.AddMediator(x =>
{
    x.ServiceLifetime = ServiceLifetime.Scoped;
    x.PipelineBehaviors = [typeof(ValidationPipelineBehaviour<,>)];
});

builder.Services.AddMcpServer().WithToolsFromAssembly().WithHttpTransport();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Use the default MCP policy name that we've configured
app.MapMcp().RequireAuthorization();

// Add a custom endpoint for the OAuth protected resource metadata
// Should be supplied by MCP nuget.
app.MapGet(
    "/.well-known/oauth-protected-resource",
    () =>
    {
        var results = new Prm
        {
            Resource = new Uri("https://localhost:7077/"),
            TokenEndpointAuthMethodsSupported = ["client_secret_basic", "client_secret_post"],
            AuthorizationServers = ["http://localhost:8081/realms/tenant-1"],
            ScopesSupported = ["openid", "profile"],
        };
        return Results.Json(results);
    }
);

await app.RunAsync();

sealed record Prm
{
    [JsonPropertyName("resource")]
    public required Uri Resource { get; init; }

    [JsonPropertyName("token_endpoint_auth_methods_supported")]
    public required List<string> TokenEndpointAuthMethodsSupported { get; init; }

    [JsonPropertyName("authorization_servers")]
    public required List<string> AuthorizationServers { get; init; }

    [JsonPropertyName("scopes_supported")]
    public required List<string> ScopesSupported { get; init; }
}
