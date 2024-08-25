# NorthernNerds.Aspire.Neo4

**NorthernNerds.Aspire.Neo4** is an **unofficial** Aspire component for integrating Neo4j in [dotnet Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview). This package simplifies working with Neo4j within Aspire-based applications by creating a Neo4j container and injecting an `IDriver` into the DI.

## Quick Start

Add the Neo4j resource in `AppHost`:

```csharp
using NorthernNerds.Aspire.Neo4;

var builder = DistributedApplication.CreateBuilder(args);

// Add Neo4j resource
var neo4jDb = builder.AddNeo4j("graph-db", "neo4j-user", "neo4j-pass");
```

Configure parameters in `appsettings.json` for local development:

```json
{
  "Parameters": {
    "neo4j-pass": "Password",
    "neo4j-user": "neo4j"
  }
}
```

Add the Neo4j client in your "client" code:

```csharp
using NorthernNerds.Aspire.Neo4;

builder.AddNeo4jClient("graph-db");
```

Inject the `IDriver` into your services:

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

For more details, visit the [GitHub repository](https://github.com/YourGitHubRepoURL).

## About Northern Nerds

Northern Nerds is a one-man freelance software company focused on delivering quality software tools. Follow our work to stay updated on the latest projects.