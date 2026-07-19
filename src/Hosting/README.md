# NorthernNerds.Aspire.Hosting.Neo4j

![Build Status](https://github.com/terle/aspire-neo4j/actions/workflows/publish.yml/badge.svg)
![NuGet Version](https://img.shields.io/nuget/v/NorthernNerds.Aspire.Hosting.Neo4j.svg)
![License](https://img.shields.io/github/license/terle/aspire-neo4j.svg)

**NorthernNerds.Aspire.Hosting.Neo4j** is an **unofficial** Aspire hosting integration for Neo4j. This package provides the infrastructure components needed to run Neo4j in your Aspire projects.

## Features
- Add Neo4j as a container resource in your Aspire AppHost
- Configure Neo4j connection settings through Aspire parameters
- Built-in HTTP health check so `WaitFor(...)` waits for Neo4j to be ready
- Data persistence via `WithDataVolume` / `WithDataBindMount`
- Automatic container management and configuration

## Release notes

### 3.1.0 (since v3.0.0)
- Updated to .NET Aspire 13.4
- Added an HTTP health check on the Neo4j endpoint so `WaitFor(...)` waits for real readiness
- Added `WithDataVolume` and `WithDataBindMount` for data persistence (mounts `/data`)
- Added argument guard clauses to `AddNeo4j`
- Bumped the Neo4j container image to the 5.26 LTS

### 3.0.0 (since v2.0.0)
- Upgraded to .NET 10 and .NET Aspire 13.1
- Enhanced `AddNeo4j` hosting integration (`[ResourceName]`, nullable ports, improved `NEO4J_AUTH` handling, endpoint refactor)
- Example updated to wait for Neo4j readiness

## Getting Started

### Installation
```bash
dotnet add package NorthernNerds.Aspire.Hosting.Neo4j
```
## Usage
In your AppHost project:
```csharp
using NorthernNerds.Aspire.Hosting.Neo4j;

var builder = DistributedApplication.CreateBuilder(args);

var neo4jPass = builder.AddParameter("neo4j-pass", secret: true);
var neo4jUser = builder.AddParameter("neo4j-user", secret: true);

var neo4jDb = builder.AddNeo4j("graph-db", neo4jUser, neo4jPass)
    .WithDataVolume(); // persist the graph across restarts

builder.AddProject<Projects.MyApi>("api")
    .WithReference(neo4jDb)
    .WaitFor(neo4jDb); // gated on the built-in Neo4j health check
```

> `AddNeo4j` registers an HTTP health check against the Neo4j endpoint, so `WaitFor(neo4jDb)`
> blocks dependents until Neo4j is actually accepting connections. Use `WithDataBindMount("./data")`
> instead of `WithDataVolume()` to persist to a host directory.

Configure parameters in `appsettings.json`:
```json
{
  "Parameters": {
    "neo4j-pass": "Password",
    "neo4j-user": "neo4j"
  }
}
```

## About Northern Nerds
Northern Nerds is a one-man freelance software company dedicated to creating high-quality software solutions. For more integrations and tools, follow our work!