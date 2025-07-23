IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EM_API>("api");

await builder.Build().RunAsync();
