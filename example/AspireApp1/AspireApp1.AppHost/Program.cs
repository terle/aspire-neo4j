using NorthernNerds.Aspire.Neo4j.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var neo4jPass = builder.AddParameter("neo4j-pass", secret: true);
var neo4jUser = builder.AddParameter("neo4j-user", secret: true);

var neo4jDb = builder.AddNeo4j("graph-db", neo4jUser, neo4jPass);

var apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(neo4jDb);

builder.Build().Run();
