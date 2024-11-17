# NorthernNerds.Aspire.Neo4j

![Build Status](https://github.com/terle/aspire-neo4j/actions/workflows/publish.yml/badge.svg)
![NuGet Version](https://img.shields.io/nuget/v/NorthernNerds.Aspire.Neo4j.svg)
![License](https://img.shields.io/github/license/terle/aspire-neo4j.svg)

**NorthernNerds.Aspire.Neo4j** is an **unofficial** Aspire client integration for Neo4j. This package provides the client-side components needed to connect to Neo4j in your Aspire services.

## Features
- Register Neo4j client in your service projects
- Automatic injection of configured `IDriver`
- Integration with Aspire service discovery

## Getting Started

### Installation
```bash
dotnet add package NorthernNerds.Aspire.Neo4j
```

## Usage
In your service project:
```csharp
using NorthernNerds.Aspire.Neo4j;

var builder = WebApplication.CreateBuilder(args);
builder.AddNeo4jClient("graph-db");
```
Use the injected `IDriver` in your services:
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
## About Northern Nerds
Northern Nerds is a one-man freelance software company dedicated to creating high-quality software solutions. For more integrations and tools, follow our work!