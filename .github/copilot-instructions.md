# Aspire Neo4j Integration - Copilot Instructions

## Project Overview

This is a **dual-package .NET Aspire integration** for Neo4j graph database:
- **NorthernNerds.Aspire.Neo4j** (`src/Client/`) - Client library for service projects
- **NorthernNerds.Aspire.Hosting.Neo4j** (`src/Hosting/`) - AppHost/orchestration library

Both packages target .NET 10.0 and must maintain **version parity** (always release together with same version).

## Architecture Patterns

### Two-Package Aspire Integration Model

Follow .NET Aspire's split-package pattern (as documented in [MS Learn Custom Components](https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/custom-component)):

**Hosting Package** (`src/Hosting/`):
- Extends `IDistributedApplicationBuilder` with `AddNeo4j()` method
- Returns `IResourceBuilder<Neo4jResource>` for fluent configuration
- Uses `Neo4jResource : ContainerResource, IResourceWithConnectionString`
- Defines container image via `Neo4jContainerImageTags` constants (registry: docker.io, image: neo4j, tag: 5.23.0-community-bullseye)
- Builds connection strings with `ReferenceExpression.Create()` for deferred resolution
- Sets `NEO4J_AUTH` environment variable as `username/password` format

**Client Package** (`src/Client/`):
- Extends `IHostApplicationBuilder` with `AddNeo4jClient()` and `AddKeyedNeo4jClient()`
- Registers Neo4j `IDriver` as singleton (or keyed singleton)
- Parses connection strings from `ConnectionStrings:<name>` or structured format: `Endpoint=neo4j://host:port;Username=user;Password=pass`
- Uses configuration section `Neo4j:Client` (defined in `Neo4jClientSettings.DefaultConfigSectionName`)
- Settings class: `Neo4jClientSettings` with `Endpoint`, `Credentials`, health check/telemetry flags

### Connection String Format

The client expects either:
1. Simple URI: `neo4j://localhost:7687`
2. Structured: `Endpoint=neo4j://host:port;Username=neo4j;Password=secret`

Parse logic in `Neo4jClientSettings.ParseConnectionString()` handles both via `DbConnectionStringBuilder`.

### Parameter Resources for Secrets

Use Aspire's `ParameterResource` pattern for credentials:
```csharp
var neo4jPass = builder.AddParameter("neo4j-pass", secret: true);
var neo4jUser = builder.AddParameter("neo4j-user", secret: true);
var neo4jDb = builder.AddNeo4j("graph-db", neo4jUser, neo4jPass);
```

Store values in AppHost's `appsettings.json` under `Parameters` section.

## File Organization

- `src/Client/` - Client library (4 files: Extensions, Settings, HealthCheck, Telemetry)
- `src/Hosting/` - Hosting library (2 files: Resource, ResourceBuilderExtensions)
- `example/AspireApp1/` - Full working example with AppHost, ApiService, and Web
- `docs/` - CONTRIBUTING.md and RELEASE_GUIDE.md
- Both packages embed `README.md` and `assets/neo4j-icon.png` in NuGet packages

## Development Workflows

### Building

```bash
dotnet restore src/NorthernNerds.Aspire.Neo4j.sln
dotnet build src/NorthernNerds.Aspire.Neo4j.sln --configuration Release
```

Packages auto-generate on build (`GeneratePackageOnBuild=true`) to `bin/Debug/` or `bin/Release/`.

### Testing Changes

Use the example app (`example/AspireApp1/`):
1. Make code changes in `src/Client/` or `src/Hosting/`
2. Build solution
3. Run `example/AspireApp1/AspireApp1.AppHost` via `dotnet run` or F5
4. Verify Neo4j container starts and Web project connects

**Reference Implementation:** `example/AspireApp1/AspireApp1.Web/Components/Pages/GraphDataBase.razor` demonstrates:
- Injecting `IDriver` via `@inject IDriver _driver`
- Using `_driver.AsyncSession()` for async queries
- Creating nodes with `tx.RunAsync()` inside `ExecuteWriteAsync()`
- Accessed at `/graph` route with streaming rendering and output caching

### Release Process

**Critical:** Both packages must share identical version numbers.

1. Update `<Version>` in both `.csproj` files:
   - `src/Client/NorthernNerds.Aspire.Neo4j.csproj`
   - `src/Hosting/NorthernNerds.Aspire.Hosting.Neo4j.csproj`

2. Commit and tag:
   ```bash
   git add src/Client/NorthernNerds.Aspire.Neo4j.csproj src/Hosting/NorthernNerds.Aspire.Hosting.Neo4j.csproj
   git commit -m "Update version to X.Y.Z"
   git tag vX.Y.Z
   git push origin vX.Y.Z
   ```

3. GitHub Actions (`.github/workflows/publish.yml`) auto-publishes to NuGet on `v*.*.*` tags

## Code Conventions

- **XML Documentation:** All public APIs require `<summary>` tags (enforced by `GenerateDocumentationFile=true`)
- **Nullable:** Enabled project-wide; use `ArgumentNullException.ThrowIfNull()` for null checks
- **Naming:** Follow Aspire patterns - `AddNeo4j`, `AddNeo4jClient`, `Neo4jResource`, `Neo4jClientSettings`
- **Constants:** Use internal static classes for grouped constants (e.g., `Neo4jContainerImageTags`)
- **Endpoint Names:** Use `const string` fields in resource classes (`BoltEndpointName`, `HttpEndpointName`)

## Integration Points

- **Aspire.Hosting** (v13.1.0) - Hosting package dependency
- **Neo4j.Driver** (v5.28.4) - Client package dependency  
- **Microsoft.Extensions.Diagnostics.HealthChecks** - Client health checks
- **OpenTelemetry** (via AspNetCore.OpenTelemetry) - Client metrics/tracing

Connection flow: AppHost `AddNeo4j()` → connection string → service project `AddNeo4jClient()` → `IDriver` singleton

## Common Tasks

**Add new configuration option:**
1. Add property to `Neo4jClientSettings` with XML docs
2. Update `ParseConnectionString()` if needed for connection string support
3. Apply in `Neo4jClientExtensions.AddNeo4jClient()` when creating `IDriver`

**Update Neo4j version:**
- Change `Neo4jContainerImageTags.Tag` constant in `src/Hosting/Neo4jResourceBuilderExtensions.cs`

**Support additional endpoints:**
- Add endpoint constant to `Neo4jResource`
- Add `WithEndpoint()` or `WithHttpEndpoint()` call in `AddNeo4j()` extension method
