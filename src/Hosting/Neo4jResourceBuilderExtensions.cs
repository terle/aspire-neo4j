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
        [ResourceName] string name,
        IResourceBuilder<ParameterResource>? neo4jUser = null,
        IResourceBuilder<ParameterResource>? neo4jPassword = null,
        int? boltPort = null,
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

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
            .WithEnvironment(context =>
            {
                // Neo4j expects `NEO4J_AUTH` as `username/password`.
                context.EnvironmentVariables[AuthEnvVarName] =
                    ReferenceExpression.Create($"{graphDb.UserNameReference}/{graphDb.PasswordParameter}");
            })
            .WithEndpoint(
                targetPort: 7687,
                port: boltPort,
                name: Neo4jResource.BoltEndpointName)
            .WithHttpEndpoint(
                targetPort: 7474,
                port: httpPort,
                name: Neo4jResource.HttpEndpointName)
            // Probe the Neo4j HTTP endpoint so WaitFor(...) only completes once the
            // server is actually accepting connections, not merely when the container starts.
            .WithHttpHealthCheck(endpointName: Neo4jResource.HttpEndpointName);
    }

    /// <summary>
    /// Adds a named volume for the data folder to a <see cref="Neo4jResource"/> so the graph
    /// database survives container restarts.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name.</param>
    /// <param name="isReadOnly">A flag that indicates if the data volume is read-only.</param>
    /// <returns>The <see cref="IResourceBuilder{Neo4jResource}"/> for further configuration.</returns>
    public static IResourceBuilder<Neo4jResource> WithDataVolume(
        this IResourceBuilder<Neo4jResource> builder,
        string? name = null,
        bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(
            name ?? VolumeNameGenerator.Generate(builder, "data"),
            Neo4jContainerImageTags.DataPath,
            isReadOnly);
    }

    /// <summary>
    /// Adds a bind mount for the data folder to a <see cref="Neo4jResource"/> so the graph
    /// database is persisted to a host directory.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="source">The source directory on the host to mount into the container.</param>
    /// <param name="isReadOnly">A flag that indicates if the data bind mount is read-only.</param>
    /// <returns>The <see cref="IResourceBuilder{Neo4jResource}"/> for further configuration.</returns>
    public static IResourceBuilder<Neo4jResource> WithDataBindMount(
        this IResourceBuilder<Neo4jResource> builder,
        string source,
        bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(source);

        return builder.WithBindMount(source, Neo4jContainerImageTags.DataPath, isReadOnly);
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

    // The tag of the Neo4j container image (5.26 is the current Neo4j 5 LTS release).
    internal const string Tag = "5.26.24-community-bullseye";

    // The in-container path where Neo4j persists its graph data.
    internal const string DataPath = "/data";
}
