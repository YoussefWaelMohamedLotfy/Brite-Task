using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(x => x.WithTheme(ScalarTheme.Default)
    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));

app.UseHttpsRedirection();

await app.RunAsync();

