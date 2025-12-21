using System.Diagnostics;
using Aspire.Hosting.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Brite_Task.AppHost;

internal static class Extensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        public IResourceBuilder<ExecutableResource> AddEfMigrate(
            IResourceBuilder<ProjectResource> app,
            IResourceBuilder<IResourceWithConnectionString> database
        )
        {
            var projectDirectory = Path.GetDirectoryName(
                app.Resource.GetProjectMetadata().ProjectPath
            )!;

            var efmigrate = builder
                .AddExecutable($"ef-migrate-{app.Resource.Name}", "dotnet", projectDirectory)
                .WithArgs("ef")
                .WithArgs("database")
                .WithArgs("update")
                .WithArgs("--no-build")
                .WithArgs("--connection")
                .WithArgs(database.Resource)
                .WithEnvironment("DOTNET_ENVIRONMENT", "Development")
                .WaitFor(database);

            efmigrate.WithPipelineStepFactory(factoryContext =>
            {
#pragma warning disable ASPIREPIPELINES001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                var step = new PipelineStep
                {
                    Name = $"ef-migration-bundle-{app.Resource.Name}",
                    Tags = [WellKnownPipelineTags.BuildCompute],
                    Action = async context =>
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = "dotnet",
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            WorkingDirectory = projectDirectory,
                        };

                        // dotnet ef migrations bundle --self-contained -r linux-x64
                        psi = psi.WithArgs(
                            "ef",
                            "migrations",
                            "bundle",
                            "--self-contained",
                            "-r",
                            "linux-x64"
                        );

                        await psi.ExecuteAsync(context.Logger, context.CancellationToken);
                    },
                };

                return [step];
            });

            efmigrate.WithPipelineConfiguration(context =>
            {
                var appContainerBuildSteps = context.GetSteps(
                    app.Resource,
                    WellKnownPipelineTags.BuildCompute
                );

                var migrationBundle = context.GetSteps(
                    efmigrate.Resource,
                    WellKnownPipelineTags.BuildCompute
                );

                appContainerBuildSteps.DependsOn(migrationBundle);
            });

            return efmigrate;
        }
#pragma warning restore ASPIREPIPELINES001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }

    extension(ProcessStartInfo psi)
    {
        public ProcessStartInfo WithArgs(params string[] args)
        {
            foreach (var arg in args)
            {
                psi.ArgumentList.Add(arg);
            }
            return psi;
        }

        // Exec with logs

        public Task<int> ExecuteAsync(ILogger logger, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<int>();

            var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    logger.LogDebug(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    logger.LogDebug(e.Data);
                }
            };

            process.Exited += (sender, e) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            cancellationToken.Register(() =>
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            });

            return tcs.Task;
        }
    }

    extension(IResourceBuilder<ProjectResource> resourceBuilder)
    {
        public IResourceBuilder<ProjectResource> WithDataPopulation()
        {
            return resourceBuilder.WithCommand(
                "seed-data",
                "Seed database with fake data using Bogus",
                async context =>
                {
                    //await SeedDatabaseAsync(resourceBuilder, context);
                    return new ExecuteCommandResult { Success = true };
                }
            );
        }
    }

    extension(IResourceBuilder<PostgresDatabaseResource> resourceBuilder)
    {
        public IResourceBuilder<PostgresDatabaseResource> WithResetDbCommand()
        {
            return resourceBuilder.WithCommand(
                "reset",
                "Reset Database",
                async context =>
                {
#pragma warning disable ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                    var interactionService =
                        context.ServiceProvider.GetRequiredService<IInteractionService>();
#pragma warning restore ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

                    var result = await interactionService.PromptConfirmationAsync(
                        "Are you sure you want to reset the database? This action cannot be undone.",
                        "Confirm Reset?"
                    );

                    if (!result.Data || result.Canceled)
                    {
                        return new ExecuteCommandResult
                        {
                            Success = false,
                            ErrorMessage = "Database reset cancelled by user.",
                        };
                    }

                    var rcs = context.ServiceProvider.GetRequiredService<ResourceCommandService>();

                    // Cheating because there's no volume, just reboot the container
                    await rcs.ExecuteCommandAsync(
                        resourceBuilder.Resource.Parent,
                        KnownResourceCommands.RestartCommand,
                        context.CancellationToken
                    );

                    // Custom reset logic if needed
                    return new ExecuteCommandResult { Success = true };
                }
            );
        }
    }
}
