using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4j.Driver;

namespace NorthernNerds.Aspire.Neo4j;

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

        var settings = new Neo4jClientSettings();

        builder.Configuration
               .GetSection(configurationSectionName)
               .Bind(settings);

        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
            settings.ParseConnectionString(connectionString);

        configureSettings?.Invoke(settings);

        if (serviceKey is null)
        {
            builder.Services.AddSingleton(CreateDriver);
        }
        else
        {
            builder.Services.AddKeyedSingleton(serviceKey, (sp, _) => CreateDriver(sp));
        }

        IDriver CreateDriver(IServiceProvider _)
        {
            ArgumentNullException.ThrowIfNull(settings.Endpoint);
            ArgumentNullException.ThrowIfNull(settings.Credentials);

            return GraphDatabase.Driver(settings.Endpoint, settings.Credentials);
        }

        if (settings.DisableHealthChecks is false)
        {
            builder.Services.AddHealthChecks()
                .AddCheck<Neo4jHealthCheck>(
                    name: serviceKey is null ? "Neo4j" : $"Neo4j_{connectionName}",
                    failureStatus: default,
                    tags: []);
        }        
    }
}
