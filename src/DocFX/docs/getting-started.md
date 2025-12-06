# Getting Started

## Running the Solution with Aspire

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/products/docker-desktop/) / Podman (for running containers)
- .NET Aspire SDK (from VS Installer, under Individual Components)

### Steps

1. **Build the Solution at the root directory**
```sh
dotnet build
```

2. **Run with Aspire**  
   Navigate to the `src/Aspire/Brite-Task.AppHost` directory and run:
```sh
dotnet run
```
   This will:
   - Start all required containers (Keycloak, Garnet, PostgreSQL)
   - Run the migration worker to apply DB migrations
   - Start the API service, wired to all dependencies
   - Start the MCP server for integration

3. **Access Services**
   - **API:** [https://localhost:7157/scalar](https://localhost:7157/scalar)
   - **Keycloak Admin:** [http://localhost:8081](http://localhost:8081)
   - **PgAdmin:** [http://localhost:5050](http://localhost:5050)

### Service Health & Observability

- Health checks are exposed at `/health` and `/alive` endpoints.
- OpenTelemetry traces and metrics are enabled for all services.