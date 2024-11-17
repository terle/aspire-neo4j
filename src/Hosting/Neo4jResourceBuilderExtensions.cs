using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace NorthernNerds.Aspire.Hosting.Neo4j;

/// <summary>
/// Provides extension methods for adding and configuring Neo4j resources in a distributed application.
/// </summary>
public static class Neo4jResourceBuilderExtensions
{
    // The environment variable name for Neo4j authentication.
    private const string AuthEnvVarName = "NEO4J_AUTH";

    /// <summary>
    /// Adds a Neo4j resource to the distributed application builder.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/> to add the resource to.</param>
    /// <param name="name">The name of the Neo4j resource.</param>
    /// <param name="neo4jUser">An optional resource builder for the Neo4j username parameter.</param>
    /// <param name="neo4jPassword">An optional resource builder for the Neo4j password parameter.</param>
    /// <param name="boltPort">The port for the Bolt protocol (default is 7687).</param>
    /// <param name="httpPort">The port for the HTTP protocol (default is 7474).</param>
    /// <returns>An <see cref="IResourceBuilder{Neo4jResource}"/> for further configuration.</returns>
    public static IResourceBuilder<Neo4jResource> AddNeo4j(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<ParameterResource>? neo4jUser = null,
        IResourceBuilder<ParameterResource>? neo4jPassword = null,
        int? boltPort = 7687,
        int? httpPort = 7474)
    {
        // Create or retrieve the password parameter.
        var passwordParameter = neo4jPassword?.Resource ??
            ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(
                builder, $"{name}-password");

        // Create a new Neo4j resource with the provided parameters.
        var graphDb = new Neo4jResource(name, neo4jUser?.Resource, passwordParameter);

        // Add the Neo4j resource to the builder and configure it.
        return builder.AddResource(graphDb)
            .WithImage(Neo4jContainerImageTags.Image)
            .WithImageRegistry(Neo4jContainerImageTags.Registry)
            .WithImageTag(Neo4jContainerImageTags.Tag)
            .WithEnvironment(AuthEnvVarName, $"{graphDb.UsernameParameter.Value}/{graphDb.PasswordParameter.Value}")
            .WithEndpoint(
                        targetPort: boltPort,
                        port: 7687,
                        name: Neo4jResource.BoltEndpointName)
            .WithHttpEndpoint(
                        targetPort: httpPort,
                        port: 7474,
                        name: Neo4jResource.HttpEndpointName);
    }
}

/// <summary>
/// Contains constants for the Neo4j container image tags.
/// </summary>
internal static class Neo4jContainerImageTags
{
    // The registry where the Neo4j container image is hosted.
    internal const string Registry = "docker.io";

    // The name of the Neo4j container image.
    internal const string Image = "neo4j";

    // The tag of the Neo4j container image.
    internal const string Tag = "5.23.0-community-bullseye";
}
