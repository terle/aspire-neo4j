# Aspire Neo4j Example

This example project demonstrates how to integrate Neo4j with an Aspire application using the NorthernNerds.Aspire.Neo4j packages:
- **NorthernNerds.Aspire.Hosting.Neo4j** - Container and resource management
- **NorthernNerds.Aspire.Neo4j** - Client configuration and dependency injection

## Getting Started

### Prerequisites
- .NET 9.0 or later
- Docker (for running Neo4j container)
- Visual Studio 2022 or VS Code

### Setup

1. Install required packages:
```bash
dotnet add package NorthernNerds.Aspire.Hosting.Neo4j
dotnet add package NorthernNerds.Aspire.Neo4j
```

2. Configure Neo4j in AppHost:
```csharp
using NorthernNerds.Aspire.Hosting.Neo4j;

var builder = DistributedApplication.CreateBuilder(args);
var neo4jDb = builder.AddNeo4j("graph-db", neo4jUser, neo4jPass);
```
4. Add client in your service:
```csharp
using NorthernNerds.Aspire.Neo4j;

var builder = WebApplication.CreateBuilder(args);
builder.AddNeo4jClient("graph-db");
```

## Example Components
### GraphDataBase.razor
A demo component showing Neo4j integration:
 - Creates and reads graph data
 - Uses streaming rendering
 - Implements output caching
 - Shows loading states

Access at `/graph` to see:
- Real-time node creation
- Data retrieval
- Graph