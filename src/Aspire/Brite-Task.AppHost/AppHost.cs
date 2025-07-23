IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

var garnet = builder.AddGarnet("garnet")
    .WithImage("microsoft/garnet-alpine")
    .WithImageTag("latest")
    .WithLifetime(ContainerLifetime.Persistent);

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("alpine")
    .WithDataVolume()
    .WithPgAdmin(x => x.WithImageTag("latest")
        .WithHostPort(5050)
        .WithLifetime(ContainerLifetime.Persistent)
    )
    .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("Employee-Management-Db");

var keycloak = builder.AddKeycloak("keycloak", 8081)
    .WithImageTag("latest")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var migrationsWorker = builder.AddProject<Projects.EM_MigrationsWorker>("migrations")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.EM_API>("api")
    .WithReference(keycloak)
    .WithReference(garnet)
    .WithReference(postgresdb)
    .WithReference(migrationsWorker)
    .WaitForCompletion(migrationsWorker);


await builder.Build().RunAsync();
