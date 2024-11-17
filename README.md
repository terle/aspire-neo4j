# NorthernNerds.Aspire.Neo4j

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
