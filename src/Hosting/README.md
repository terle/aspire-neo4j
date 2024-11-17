# NorthernNerds.Aspire.Hosting.Neo4j

![Build Status](https://github.com/terle/aspire-neo4j/actions/workflows/publish.yml/badge.svg)
![NuGet Version](https://img.shields.io/nuget/v/NorthernNerds.Aspire.Hosting.Neo4j.svg)
![License](https://img.shields.io/github/license/terle/aspire-neo4j.svg)

**NorthernNerds.Aspire.Hosting.Neo4j** is an **unofficial** Aspire hosting integration for Neo4j. This package provides the infrastructure components needed to run Neo4j in your Aspire projects.

## Features
- Add Neo4j as a container resource in your Aspire AppHost
- Configure Neo4j connection settings through Aspire parameters
- Automatic container management and configuration

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

var neo4jDb = builder.AddNeo4j("graph-db", neo4jUser, neo4jPass);
```

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