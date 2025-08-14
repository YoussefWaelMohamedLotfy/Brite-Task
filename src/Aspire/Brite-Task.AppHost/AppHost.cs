IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("env");

// The values for these parameters can be set via appsettings.json
var adminPassword = builder.AddParameter("password", secret: true);

var garnet = builder.AddGarnet("garnet", password: adminPassword)
    .WithImage("microsoft/garnet-alpine")
    .WithImageTag("latest")
    .WithLifetime(ContainerLifetime.Persistent);

var postgres = builder.AddPostgres("postgres", password: adminPassword)
    .WithImageTag("alpine")
    .WithDataVolume()
    .WithPgAdmin(x => x.WithImageTag("latest")
        .WithHostPort(5050)
        .WithLifetime(ContainerLifetime.Persistent)
    )
    .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("Employee-Management-Db");

var keycloak = builder.AddKeycloak("keycloak", 8081, adminPassword: adminPassword)
    .WithImageTag("latest")
    .WithDataVolume()
    .WithRealmImport("./Realms")
    .WithLifetime(ContainerLifetime.Persistent);

var migrationsWorker = builder.AddProject<Projects.EM_MigrationsWorker>("migrations")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

var api = builder.AddProject<Projects.EM_API>("api")
    .WithReference(keycloak)
    .WithReference(garnet)
    .WithReference(postgresdb)
    .WithReference(migrationsWorker)
    .WaitForCompletion(migrationsWorker);

var mcpServer = builder.AddProject<Projects.EM_McpServer>("mcpserver")
    .WithReference(keycloak)
    .WithReference(postgresdb)
    .WithReference(migrationsWorker)
    .WaitForCompletion(migrationsWorker);

builder.AddMcpInspector("mcp-inspector", inspectorVersion: "0.16.3")
    .WithMcpServer(mcpServer);

builder.AddProject<Projects.EM_Blazor>("blazor")
    .WithReference(api)
    .WithReference(mcpServer)
    .WithReference(keycloak)
    .WaitFor(api);

await builder.Build().RunAsync();
