using Aspire.Hosting.ApplicationModel;

namespace NorthernNerds.Aspire.Neo4jComponent.Hosting;

/// <summary>
/// Represents a Neo4j resource within the Aspire hosting environment.
/// </summary>
/// <param name="name">The name of the Neo4j resource.</param>
/// <param name="username">The parameter resource for the Neo4j server username.</param>
/// <param name="password">The parameter resource for the Neo4j server password.</param>
public sealed class Neo4jResource(string name, ParameterResource? username, ParameterResource password) : ContainerResource(name), IResourceWithConnectionString
{
    // The name of the Bolt protocol endpoint.
    internal const string BoltEndpointName = "neo4j";

    // The name of the HTTP protocol endpoint.
    internal const string HttpEndpointName = "http";

    // References to the Bolt and HTTP endpoints.
    private EndpointReference? _boltEndpointReference;
    private EndpointReference? _httpEndpointReference;

    // The default username for the Neo4j server.
    private const string DefaultUsername = "neo4j";

    /// <summary>
    /// Gets the parameter that contains the Neo4j server username.
    /// </summary>
    public ParameterResource? UsernameParameter { get; } = username;

    /// <summary>
    /// Gets the parameter that contains the Neo4j server password.
    /// </summary>
    public ParameterResource PasswordParameter { get; } = password;

    /// <summary>
    /// Gets the reference expression for the username.
    /// If the username parameter is not provided, the default username is used.
    /// </summary>
    internal ReferenceExpression UserNameReference =>
       UsernameParameter is not null ?
       ReferenceExpression.Create($"{UsernameParameter}") :
       ReferenceExpression.Create($"{DefaultUsername}");

    /// <summary>
    /// Gets the reference to the Bolt protocol endpoint.
    /// </summary>
    public EndpointReference BoltEndpoint =>
        _boltEndpointReference ??= new EndpointReference(this, BoltEndpointName);

    /// <summary>
    /// Gets the reference to the HTTP protocol endpoint.
    /// </summary>
    public EndpointReference HttpEndpoint =>
       _httpEndpointReference ??= new(this, HttpEndpointName);

    /// <summary>
    /// Gets the connection string expression for the Neo4j resource.
    /// </summary>
    public ReferenceExpression ConnectionStringExpression =>
    ReferenceExpression.Create(
            $"Endpoint=neo4j://{BoltEndpoint.Property(EndpointProperty.Host)}:{BoltEndpoint.Property(EndpointProperty.Port)};Username={UserNameReference};Password={PasswordParameter}"
    );
}
