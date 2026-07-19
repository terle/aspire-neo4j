# Neo4j Aspire Integration (Unofficial)

![Build Status](https://github.com/terle/aspire-neo4j/actions/workflows/publish.yml/badge.svg)

This repository provides **unofficial** Neo4j integrations for [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview).

## Packages

| Package | Version | Downloads |
|---------|---------|-----------|
| NorthernNerds.Aspire.Neo4j | ![NuGet](https://img.shields.io/nuget/v/NorthernNerds.Aspire.Neo4j.svg) | ![Downloads](https://img.shields.io/nuget/dt/NorthernNerds.Aspire.Neo4j.svg) |
| NorthernNerds.Aspire.Hosting.Neo4j | ![NuGet](https://img.shields.io/nuget/v/NorthernNerds.Aspire.Hosting.Neo4j.svg) | ![Downloads](https://img.shields.io/nuget/dt/NorthernNerds.Aspire.Hosting.Neo4j.svg) |

## Getting Started

### Installation

```bash
# Install both packages
dotnet add package NorthernNerds.Aspire.Hosting.Neo4j
dotnet add package NorthernNerds.Aspire.Neo4j
```

## Usage
In your AppHost project:
```csharp
using NorthernNerds.Aspire.Hosting.Neo4j;

var builder = DistributedApplication.CreateBuilder(args);
var neo4jDb = builder.AddNeo4j("graph-db", neo4jUser, neo4jPass);
```
In your service projects:
```csharp
using NorthernNerds.Aspire.Neo4j;

var builder = WebApplication.CreateBuilder(args);
builder.AddNeo4jClient("graph-db");
```
For more details, check the:

- [Example Project](example/README.md)
- [Client Documentation](src/Client/README.md)
- [Hosting Documentation](src/Hosting/README.md)


## Documentation

- [Release Guide](docs/RELEASE_GUIDE.md): Instructions on how to release a new version of the package.

## Contributing

Contributions are welcome! Please see the [CONTRIBUTING.md](docs/CONTRIBUTING.md) for details on how to contribute.

## License

This project is licensed under the MIT License.

## About Northern Nerds

Northern Nerds is a one-man freelance software company dedicated to creating high-quality software solutions and tools. Follow our work to stay updated on the latest projects and contributions.

## Release notes

### 3.1.0 (since v3.0.0)

**NorthernNerds.Aspire.Neo4j (Client)**
- Updated for the .NET Aspire 13.4 release
- Removed the unused `OpenTelemetry.Extensions.Hosting` dependency

**NorthernNerds.Aspire.Hosting.Neo4j (Hosting)**
- Updated to .NET Aspire 13.4
- Added an HTTP health check on the Neo4j endpoint so `WaitFor(...)` waits for real readiness
- Added `WithDataVolume` and `WithDataBindMount` for data persistence (mounts `/data`)
- Added argument guard clauses to `AddNeo4j`
- Bumped the Neo4j container image to the 5.26 LTS

Thanks to [@joshmakestuff](https://github.com/joshmakestuff) for this contribution! ([#7](https://github.com/terle/aspire-neo4j/pull/7))

**Example app**
- Added an interactive Movies sample (`/movies`) demonstrating cast/director queries and a "six degrees of co-stars" shortest-path search, replacing the old trivial demo as the featured example

Thanks again to [@joshmakestuff](https://github.com/joshmakestuff) for this contribution! ([#8](https://github.com/terle/aspire-neo4j/pull/8))

### 3.0.0 (since v2.0.0)

**NorthernNerds.Aspire.Neo4j (Client)**
- Upgraded to .NET 10 and .NET Aspire 13.1
- Added health checks (enabled by default; configurable via `Neo4jClientSettings.DisableHealthChecks`)
- Improved DI registration to support multiple connections (singleton/keyed singleton)

**NorthernNerds.Aspire.Hosting.Neo4j (Hosting)**
- Upgraded to .NET 10 and .NET Aspire 13.1
- Enhanced `AddNeo4j` hosting integration (`[ResourceName]`, nullable ports, improved `NEO4J_AUTH` handling, endpoint refactor)
- Example updated to wait for Neo4j readiness