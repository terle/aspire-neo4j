# Neo4j Aspire Component

**NorthernNerds.Aspire.Neo4j** is an **unofficial** Aspire component for integrating Neo4j in [dotnet Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview). 

This package simplifies working with Neo4j within Aspire-based applications by creating a Neo4j container and injecting an `IDriver` in the DI from an officially supported [.Net driver](https://neo4j.com/docs/getting-started/languages-guides/neo4j-dotnet/) from Neo4j.

## Usage

Here’s a quick example of how to use the `NorthernNerds.Aspire.Neo4j` in your project:

In AppHost, add the resource:
```csharp
using NorthernNerds.Aspire.Neo4j;

var builder = DistributedApplication.CreateBuilder(args);

//username and password are secrets
var neo4jPass = builder.AddParameter("neo4j-pass", secret: true);
var neo4jUser = builder.AddParameter("neo4j-user", secret: true);

//add neo4j resource
var neo4jDb = builder.AddNeo4j("graph-db", neo4jUser, neo4jPass);
```

Then in "client" code, add the Neo4j client:

```csharp
using NorthernNerds.Aspire.Neo4j;

builder.AddNeo4jClient("graph-db");
```

This will registre an `IDriver` as singleton in the service collectionm, which can be injected into services.

```csharp
using Neo4j.Driver;

public class MyService(IDriver driver)
{
	public async Task DoSomething()
	{
		var session = driver.AsyncSession();
		await session.RunAsync("MATCH (n) RETURN n");
	}
}
```

## About Northern Nerds

Northern Nerds is a one-man freelance software company dedicated to creating high-quality software solutions and tools. Follow our work to stay updated on the latest projects and contributions.