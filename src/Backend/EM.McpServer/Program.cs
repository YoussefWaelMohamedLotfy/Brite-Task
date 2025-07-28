using System.Security.Claims;
using System.Text.Json;

using EM.Application;
using EM.Application.Features.Common.Behaviours;
using EM.Infrastructure.Data;
using EM.Infrastructure.Interceptors;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using ModelContextProtocol.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddKeycloakJwtBearer("keycloak", "tenant-1", JwtBearerDefaults.AuthenticationScheme, options =>
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
        RoleClaimType = "roles"
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)context.Principal!.Identity!;
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

            var name = context.Principal?.Identity?.Name ?? "unknown";
            var email = context.Principal?.FindFirstValue("preferred_username") ?? "unknown";
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
        }
    };
})
.AddMcp(options =>
{
    options.ResourceMetadataUri = new Uri("http://localhost:8081/realms/tenant-1/.well-known/openid-configuration");
    options.ResourceMetadata = new()
    {
        Resource = new Uri("https://localhost:7077/"),
        ResourceDocumentation = new Uri("https://docs.example.com/api/weather"),
        AuthorizationServers = { new Uri("http://localhost:8081/realms/tenant-1") },
        ScopesSupported = ["openid", "profile"],
    };
});

builder.Services.AddAuthorizationBuilder();

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.AddInterceptors(sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>());
    options.UseNpgsql(builder.Configuration.GetConnectionString("Employee-Management-Db"))
    .UseAsyncSeeding(async (context, _, ct) => await AppDbContextInitializer.SeedInitialDataAsync((AppDbContext)context, ct))
    .UseSeeding((context, _) => AppDbContextInitializer.SeedInitialData((AppDbContext)context));
});
builder.EnrichNpgsqlDbContext<AppDbContext>();

builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<IApplicationAssemblyMarker>()
    .AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>)));

builder.Services.AddHttpContextAccessor();
builder.Services.AddMcpServer()
    .WithToolsFromAssembly()
    .WithHttpTransport();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Use the default MCP policy name that we've configured
app.MapMcp()
    .RequireAuthorization();

await app.RunAsync();
