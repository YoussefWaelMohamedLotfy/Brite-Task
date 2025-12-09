using Brite_Task.AppHost;

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
    .WithRealmImport("./Realms")
    .WithArgs("--features=docker,admin-fine-grained-authz,token-exchange,quick-theme")
    .WithLifetime(ContainerLifetime.Persistent);

//var migrationsWorker = builder
//    .AddProject<Projects.EM_MigrationsWorker>("migrations")
//    .WithReference(postgresdb)
//    .WaitFor(postgresdb);

var api = builder
    .AddProject<Projects.EM_API>("api")
    .WithReference(keycloak)
    .WithReference(cache)
    //.WithReference(migrationsWorker)
    //.WaitForCompletion(migrationsWorker)
    .WithReference(postgresdb);

var efmigrate = builder.AddEfMigrate(api, postgresdb);

// Ensure the api is built before running
api.WaitForCompletion(efmigrate);
api.WithChildRelationship(efmigrate);
api.WithDataPopulation();

var mcpServer = builder
    .AddProject<Projects.EM_McpServer>("mcpserver")
    .WithReference(keycloak)
    .WithReference(postgresdb)
    //.WithReference(migrationsWorker)
    .WaitForCompletion(efmigrate);

builder
    .AddMcpInspector("mcp-inspector", new McpInspectorOptions() { InspectorVersion = "0.17.2" })
    .WithMcpServer(mcpServer);

var yarp = builder
    .AddProject<Projects.EM_YARP>("reverse-proxy")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder
    .AddProject<Projects.EM_Blazor>("blazor")
    .WithReference(yarp)
    .WithReference(mcpServer)
    .WithReference(keycloak)
    .WaitFor(yarp);

await builder.Build().RunAsync();
