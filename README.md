# Employee Management System - Brite Task

## Overview

This is my implementation for Brite Task, which  is an Employee Management System, built with .NET 9, leveraging the .NET Aspire stack for cloud-native development and enhanced developer experience. The solution is modular, scalable, and designed for robust service orchestration, observability, and resilience.

### Solution Structure

- **Backend Services**
  - `EM.API`: Main API for employee management.
  - `EM.Application`: Application layer for business logic.
  - `EM.Infrastructure`: Data access and infrastructure concerns.
  - `EM.Domain`: Domain models and core business rules.
  - `EM.MigrationsWorker`: Worker service for database migrations, used in Development with .NET Aspire only.

- **Aspire Projects**
  - `Brite-Task.AppHost`: Orchestrates services and dependencies using Aspire.
  - `Brite-Task.ServiceDefaults`: Shared project for service defaults (health checks, OpenTelemetry, service discovery).

## Architecture

The solution follows a layered architecture, with clear separation of concerns. Aspire is used for service orchestration, container management, and dependency wiring.

```mermaid
flowchart TD
    subgraph .NET Aspire Orchestration
        A[API Service]
        B[Migrations Worker]
        C[Keycloak Auth]
        D[Garnet Cache]
        E[Postgres DB]

    A -- Uses --> C
    A -- Uses --> D
    A -- Uses --> E
    B -- Migrates --> E
    AppHost -- Orchestrates --> A
    AppHost -- Orchestrates --> B
    AppHost -- Orchestrates --> C
    AppHost -- Orchestrates --> D
    AppHost -- Orchestrates --> E
    end
```

## Key Decisions

Here are some key decisions made during the development of this assessment, along with their rationale:

### Why choose .NET Aspire over Docker Compose?

.NET Aspire was chosen over Docker Compose primarily for its enhanced developer experience. While Docker Compose is excellent for orchestrating containers, .NET Aspire significantly streamlines the inner development loop for modern, distributed applications, especially within the .NET ecosystem. It simplifies service discovery, manages connection strings automatically, and provides an integrated dashboard for observability and debugging, all configured with C# code. This reduces boilerplate and configuration overhead, allowing developers to focus more on business logic and less on infrastructure setup, leading to faster development cycles and improved productivity compared to manual Docker Compose configurations and troubleshooting.

### Why choose PostgreSQL over SQL Server?

While SQL Server is a strong contender, especially within the Microsoft ecosystem, PostgreSQL was chosen for its compelling advantages in performance and industry adoption. PostgreSQL is renowned for its robust performance, particularly in handling complex queries and large datasets, often outperforming SQL Server in specific benchmarks. Furthermore, its open-source nature has led to widespread adoption across various industries and platforms, fostering a large and active community, extensive tooling, and broad cloud provider support. This provides greater flexibility, cost efficiency (due to no licensing fees), and future-proofing for the solution.

### Why choose Microsoft Garnet over Redis?

Microsoft Garnet was selected over Redis primarily due to its superior throughput capabilities. Garnet, a modern, open-source, and high-performance in-memory key-value store from Microsoft Research, has demonstrated significantly higher throughput and better scalability with many client connections, particularly for small batch operations. This performance edge can lead to substantial cost savings for large-scale applications and services by requiring fewer resources to handle the same load, while still being compatible with existing Redis clients.

### Why use .NET Minimal APIs over MVC Controllers?

.NET Minimal APIs were chosen over traditional MVC Controllers for their modern approach and performance benefits. Minimal APIs offer a streamlined, lightweight way to build HTTP APIs with significantly less boilerplate code, making them faster to write and easier to read. They are inherently more performant due to their leaner pipeline and reduced overhead, making them ideal for microservices and high-performance scenarios. This choice aligns with modern architectural patterns and allows for a more concise and efficient development of API endpoints.

### Why use Keycloak instead of building my own Auth Solution?

Keycloak was chosen over building a custom authentication solution due to its comprehensive features, robust security compliance, and reduced development overhead. Building a secure and feature-rich authentication and authorization system from scratch is a complex and time-consuming endeavor, fraught with potential security vulnerabilities. Keycloak, as an open-source Identity and Access Management (IAM) solution, provides out-of-the-box support for industry standards like OAuth 2.0, OpenID Connect, and SAML 2.0, along with features like multi-factor authentication, user federation, single sign-on (SSO), Multi-Tenants by design and fine-grained access control. This ensures high security compliance and allows developers to focus on core business logic rather than reinventing the wheel for security.

## Running the Solution with Aspire

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Docker / Podman (for running containers)

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

3. **Access Services**
   - **API:** `http://localhost:7157/scalar`
   - **Keycloak Admin:** `http://localhost:8081`
   - **PgAdmin:** `http://localhost:5050`

### Service Health & Observability

- Health checks are exposed at `/health` and `/alive` endpoints.
- OpenTelemetry traces and metrics are enabled for all services.

## Additional Diagrams

### Layered Architecture

```mermaid
graph TD
    Domain[Domain Layer]
    Application[Application Layer]
    Infrastructure[Infrastructure Layer]
    API[API Layer]
    API --> Application
    Application --> Domain
    Application --> Infrastructure
    Infrastructure --> Database[(PostgreSQL)]
```

### Service Orchestration

```mermaid
flowchart LR
    AppHost --> API
    AppHost --> MigrationsWorker
    AppHost --> Keycloak
    AppHost --> Garnet
    AppHost --> Postgres
    API --> Keycloak
    API --> Garnet
    API --> Postgres
    MigrationsWorker --> Postgres
```

## Aspire Dashboard

After running the solution, you can navigate to the Aspire dashboard at [https://localhost:17256/](http://localhost:17256/)

This dashboard provides a visual overview of all running services, dependencies, health, and telemetry information.

## References

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [OpenTelemetry for .NET](https://opentelemetry.io/docs/instrumentation/net/)
- [Aspire Service Defaults](https://aka.ms/dotnet/aspire/service-defaults)

---


