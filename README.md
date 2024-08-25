# NorthernNerds.Aspire.Neo4

![Build Status](https://github.com/terle/aspire-neo4j/actions/workflows/publish.yml/badge.svg)
![NuGet Version](https://img.shields.io/nuget/v/NorthernNerds.Aspire.Neo4j.svg)
![NuGet Downloads](https://img.shields.io/nuget/dt/NorthernNerds.Aspire.Neo4j.svg)
![License](https://img.shields.io/github/license/terle/aspire-neo4j.svg)
![GitHub Issues](https://img.shields.io/github/issues/terle/aspire-neo4j.svg)
![GitHub Stars](https://img.shields.io/github/stars/terle/aspire-neo4j.svg)

**NorthernNerds.Aspire.Neo4** is an **unofficial** Aspire component for integrating Neo4j into [dotnet Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview). This package simplifies working with Neo4j within Aspire-based applications by creating a Neo4j container and injecting an `IDriver` in the DI from the officially supported [.Net driver](https://neo4j.com/docs/getting-started/languages-guides/neo4j-dotnet/) by Neo4j.

## Features
- Seamless integration of Neo4j with Aspire.
- Supports adding Neo4j as a resource and injecting `IDriver` into your services.
- Simple setup for local development with easy configuration using `appsettings.json`.

## Why This Package?

This package is my first contribution to the developer community. I created it because I needed an easy way to integrate Neo4j with the Aspire framework, and I wanted to share this solution with others who might have similar needs.

I believe in the potential of Aspire and love working with graph databases. By sharing this package, I hope to make it easier for other developers to include Neo4j in their projects, and to contribute to the growth of the Aspire ecosystem.

If you find this package useful, I would love to hear from you. Contributions, suggestions, and feedback are always welcome!

Let's build something great together!

## Getting Started

### Prerequisites

- .NET 8.0 or later
- Basic knowledge of Aspire and Neo4j

### Installation

You can install this package via NuGet:

```bash
dotnet add package NorthernNerds.Aspire.Neo4
```

### Example Project

For a practical implementation of this package, check out the [Example Project](example) included in this repository. It demonstrates how to set up and use `NorthernNerds.Aspire.Neo4` in a real-world scenario.

## Usage

Here’s a quick example of how to use `NorthernNerds.Aspire.Neo4` in your project:

In `AppHost`, add the resource:

```csharp
using NorthernNerds.Aspire.Neo4;

var builder = DistributedApplication.CreateBuilder(args);

// username and password are secrets
var neo4jPass = builder.AddParameter("neo4j-pass", secret: true);
var neo4jUser = builder.AddParameter("neo4j-user", secret: true);

// add Neo4j resource
var neo4jDb = builder.AddNeo4j("graph-db", neo4jUser, neo4jPass);
```

For local development, add the parameters to `appsettings.json`:

```json
{
  "Parameters": {
    "neo4j-pass": "Password",
    "neo4j-user": "neo4j"
  }
}
```

Then in the "client" code, add the Neo4j client:

```csharp
using NorthernNerds.Aspire.Neo4;

builder.AddNeo4jClient("graph-db");
```

This will register an `IDriver` as a singleton in the service collection, which can be injected into your services.

```csharp
using Neo4j.Driver;

public class MyService
{
    private readonly IDriver _driver;

    public MyService(IDriver driver)
    {
        _driver = driver;
    }

    public async Task DoSomething()
    {
        var session = _driver.AsyncSession();
        await session.RunAsync("MATCH (n) RETURN n");
    }
}
```

## Documentation

- [Release Guide](docs/RELEASE_GUIDE.md): Instructions on how to release a new version of the package.

## Contributing

Contributions are welcome! Please see the [CONTRIBUTING.md](docs/CONTRIBUTING.md) for details on how to contribute.

## License

This project is licensed under the MIT License.

## About Northern Nerds

Northern Nerds is a one-man freelance software company dedicated to creating high-quality software solutions and tools. Follow our work to stay updated on the latest projects and contributions.