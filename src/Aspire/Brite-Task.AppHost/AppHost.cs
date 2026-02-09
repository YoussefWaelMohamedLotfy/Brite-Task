using System.Text;
using Brite_Task.AppHost;
using CliWrap;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("env");

// The values for these parameters can be set via appsettings.json
var adminPassword = builder.AddParameter("password", secret: true);

bool useGarnet = false;
IResourceBuilder<IResourceWithConnectionString> cache;

cache = useGarnet
    ? builder
        .AddGarnet("cache", password: adminPassword)
        .WithImage("microsoft/garnet-alpine")
        .WithImageTag("latest")
    : builder.AddRedis("cache", password: adminPassword).WithImageTag("alpine");

var postgres = builder
    .AddPostgres("postgres", password: adminPassword, port: 5432)
    .WithImageTag("alpine")
    .WithDataVolume()
    .WithPgAdmin(x =>
        x.WithImageTag("latest").WithHostPort(5050).WithLifetime(ContainerLifetime.Persistent)
    )
    .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("Employee-Management-Db").WithResetDbCommand();

builder
    .AddContainer("postgres-mcp", "crystaldba/postgres-mcp")
    .WithHttpEndpoint(port: 8082, targetPort: 8000)
    .WithEnvironment("DATABASE_URI", postgresdb.Resource.UriExpression)
    .WithArgs("--access-mode=unrestricted")
    .WithArgs("--transport=sse")
    .WaitFor(postgresdb)
    .WithParentRelationship(postgres)
    .WithIconName("WindowDevTools")
    .ExcludeFromManifest();

var keycloak = builder
    .AddKeycloak("keycloak", 8081, adminPassword: adminPassword)
    .WithImageTag("latest")
    .WithDataVolume()
    .WithOtlpExporter()
    .WithExternalHttpEndpoints()
    .WithRealmImport("./Realms")
    .WithArgs("--features=docker,admin-fine-grained-authz,token-exchange,quick-theme")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithUrls(context =>
    {
        foreach (var u in context.Urls)
        {
            u.DisplayLocation = UrlDisplayLocation.DetailsOnly;
        }

        // Only show the /scalar URL in the UI
        context.Urls.Add(
            new ResourceUrlAnnotation()
            {
                Url = "/",
                DisplayText = "Admin Dashboard",
                Endpoint = context.GetEndpoint("http"),
            }
        );
    });

//var migrationsWorker = builder
//    .AddProject<Projects.EM_MigrationsWorker>("migrations")
//    .WithReference(postgresdb)
//    .WaitFor(postgresdb);

var api = builder
    .AddProject<EM_API>("api")
    .WithReference(keycloak)
    .WithReference(cache)
    //.WaitForCompletion(migrationsWorker)
    //.WithChildRelationship(migrationsWorker)
    .WithReference(postgresdb)
    .WithHttpCommand(
        path: "/database/reset",
        displayName: "Reset EF Core Db Migrations",
        commandOptions: new HttpCommandOptions()
        {
            Method = HttpMethod.Post,
            Description = """
            Resets the EF Core database by dropping and recreating it.
            This command is useful for development and testing purposes, allowing you to quickly reset the database to a clean state.
            Use with caution, as it will permanently delete all data in the database.
            """,
            IconName = "ArrowReset",
            IconVariant = IconVariant.Regular,
            //IsHighlighted = true
        }
    );

var efmigrate = builder
    .AddEfMigrate(api, postgresdb)
    .WithCommand(
        "install-dotnet-ef-global-tool",
        "Install 'dotnet ef' tool globally",
        async (context) =>
        {
            StringBuilder sb = new();
            var result = await Cli.Wrap("dotnet")
                .WithArguments(["tool", "install", "-g", "dotnet-ef"])
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb))
                .ExecuteAsync(context.CancellationToken);

            return !result.IsSuccess
                ? CommandResults.Failure(sb.ToString())
                : CommandResults.Success();
        },
        new() { }
    )
    .WithCommand(
        "update-dotnet-ef-global-tool",
        "Update 'dotnet ef' tool globally",
        async (context) =>
        {
            StringBuilder sb = new();
            var result = await Cli.Wrap("dotnet")
                .WithArguments(["tool", "update", "-g", "dotnet-ef"])
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb))
                .ExecuteAsync(context.CancellationToken);

            return !result.IsSuccess
                ? CommandResults.Failure(sb.ToString())
                : CommandResults.Success();
        },
        new() { }
    );

// Ensure the api is built before running
api.WaitForCompletion(efmigrate);
api.WithChildRelationship(efmigrate);
api.WithDataPopulation();

var mcpServer = builder
    .AddProject<EM_McpServer>("mcpserver")
    .WithReference(keycloak)
    .WithReference(postgresdb)
    //.WithReference(migrationsWorker)
    .WaitForCompletion(efmigrate);

builder
    .AddMcpInspector("mcp-inspector", new McpInspectorOptions() { InspectorVersion = "0.17.2" })
    .WithMcpServer(mcpServer);

var yarp = builder
    .AddProject<EM_YARP>("reverse-proxy")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder
    .AddProject<EM_Blazor>("blazor")
    .WithReference(yarp)
    .WithReference(mcpServer)
    .WithReference(keycloak)
    .WaitFor(yarp);

await builder.Build().RunAsync();
