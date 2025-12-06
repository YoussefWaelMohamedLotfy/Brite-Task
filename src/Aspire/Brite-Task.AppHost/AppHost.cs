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
    .AddPostgres("postgres", password: adminPassword)
    .WithImageTag("alpine")
    .WithDataVolume()
    .WithPgAdmin(x =>
        x.WithImageTag("latest").WithHostPort(5050).WithLifetime(ContainerLifetime.Persistent)
    )
    .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("Employee-Management-Db");

var keycloak = builder
    .AddKeycloak("keycloak", 8081, adminPassword: adminPassword)
    .WithImageTag("latest")
    .WithDataVolume()
    .WithRealmImport("./Realms")
    .WithArgs("--features=docker,admin-fine-grained-authz,token-exchange,quick-theme")
    .WithLifetime(ContainerLifetime.Persistent);

var migrationsWorker = builder
    .AddProject<Projects.EM_MigrationsWorker>("migrations")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

var api = builder
    .AddProject<Projects.EM_API>("api")
    .WithReference(keycloak)
    .WithReference(cache)
    .WithReference(postgresdb)
    .WithReference(migrationsWorker)
    .WaitForCompletion(migrationsWorker);

var mcpServer = builder
    .AddProject<Projects.EM_McpServer>("mcpserver")
    .WithReference(keycloak)
    .WithReference(postgresdb)
    .WithReference(migrationsWorker)
    .WaitForCompletion(migrationsWorker);

builder
    .AddMcpInspector("mcp-inspector", new McpInspectorOptions() { InspectorVersion = "0.17.2" })
    .WithMcpServer(mcpServer);

builder
    .AddProject<Projects.EM_Blazor>("blazor")
    .WithReference(api)
    .WithReference(mcpServer)
    .WithReference(keycloak)
    .WaitFor(api);

builder
    .AddProject<Projects.EM_YARP>("reverse-proxy")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
