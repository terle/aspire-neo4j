# NorthernNerds.Aspire.Neo4

**NorthernNerds.Aspire.Neo4** is an **unofficial** Aspire component for integrating Neo4j into [dotnet Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview). This package simplifies working with Neo4j within Aspire-based applications by creating a Neo4j container and injecting an `IDriver` in the DI from the officially supported [.Net driver](https://neo4j.com/docs/getting-started/languages-guides/neo4j-dotnet/) by Neo4j.

## Features
- Seamless integration of Neo4j with Aspire.
- Supports adding Neo4j as a resource and injecting `IDriver` into your services.
- Simple setup for local development with easy configuration using `appsettings.json`.

## Installation

You can install this package via NuGet:

```bash
dotnet add package NorthernNerds.Aspire.Neo4
```

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

## Contributing

Contributions are welcome! Please see the [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## About Northern Nerds

Northern Nerds is a one-man freelance software company dedicated to creating high-quality software solutions and tools. Follow our work to stay updated on the latest projects and contributions.