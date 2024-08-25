using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4j.Driver;

namespace NorthernNerds.Aspire.Neo4j.Client;

/// <summary>
/// Provides extension methods for registering Neo4j client services.
/// </summary>
public static class Neo4jClientExtensions
{
    // Note: Heavily inspired by: https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/custom-component

    /// <summary>
    /// Registers a singleton <see cref="IDriver"/> to interact with the Neo4j database.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHostApplicationBuilder"/> to read configuration from and add services to.
    /// </param>
    /// <param name="connectionName">
    /// A name used to retrieve the connection string from the ConnectionStrings configuration section.
    /// </param>
    /// <param name="configureSettings">
    /// An optional delegate that can be used for customizing options.
    /// It's invoked after the settings are read from the configuration.
    /// </param>
    public static void AddNeo4jClient(this IHostApplicationBuilder builder, string connectionName, Action<Neo4jClientSettings>? configureSettings = null) => builder.AddNeo4jClient(Neo4jClientSettings.DefaultConfigSectionName, configureSettings, connectionName, serviceKey: null);

    /// <summary>
    /// Registers a singleton <see cref="IDriver"/> to interact with the Neo4j database using a keyed service.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHostApplicationBuilder"/> to read configuration from and add services to.
    /// </param>
    /// <param name="name">
    /// The name of the component, which is used as the <see cref="ServiceDescriptor.ServiceKey"/> of the
    /// service and also to retrieve the connection string from the ConnectionStrings configuration section.
    /// </param>
    /// <param name="configureSettings">
    /// An optional method that can be used for customizing options. It's invoked after the settings are
    /// read from the configuration.
    /// </param>
    public static void AddKeyedNeo4jClient(this IHostApplicationBuilder builder, string name, Action<Neo4jClientSettings>? configureSettings = null)
    {
        ArgumentNullException.ThrowIfNull(name);

        builder.AddNeo4jClient(
            $"{Neo4jClientSettings.DefaultConfigSectionName}:{name}",
            configureSettings,
            connectionName: name,
            serviceKey: name);
    }

    /// <summary>
    /// Internal method to register a singleton <see cref="IDriver"/> to interact with the Neo4j database.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHostApplicationBuilder"/> to read configuration from and add services to.
    /// </param>
    /// <param name="configurationSectionName">
    /// The name of the configuration section to read settings from.
    /// </param>
    /// <param name="configureSettings">
    /// An optional delegate that can be used for customizing options.
    /// It's invoked after the settings are read from the configuration.
    /// </param>
    /// <param name="connectionName">
    /// A name used to retrieve the connection string from the ConnectionStrings configuration section.
    /// </param>
    /// <param name="serviceKey">
    /// An optional key to register the service with.
    /// </param>
    private static void AddNeo4jClient(this IHostApplicationBuilder builder, string configurationSectionName, Action<Neo4jClientSettings>? configureSettings, string connectionName, object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Create a new instance of Neo4jClientSettings to hold the configuration settings.
        var settings = new Neo4jClientSettings();

        // Bind the configuration section to the settings object.
        builder.Configuration
               .GetSection(configurationSectionName)
               .Bind(settings);

        // Retrieve the connection string from the configuration and parse it.
        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
        {
            settings.ParseConnectionString(connectionString);
        }

        // Invoke the optional configuration delegate to customize settings.
        configureSettings?.Invoke(settings);

        // Ensure that credentials are provided.
        ArgumentNullException.ThrowIfNull(settings.Credentials);

        // Create a Neo4j driver instance with the configured endpoint and credentials.
        var driver = GraphDatabase.Driver(settings.Endpoint, settings.Credentials);

        // Register the driver as a singleton service.
        builder.Services.AddSingleton(driver);
    }
}
