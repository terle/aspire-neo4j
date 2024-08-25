using Neo4j.Driver;
using System.Data.Common;

namespace NorthernNerds.Aspire.Neo4j.Client;

// Note: Heavily inspired by: https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/custom-component

/// <summary>
/// Represents the settings required to configure a Neo4j client.
/// </summary>
public sealed class Neo4jClientSettings
{
    // The default configuration section name for Neo4j client settings.
    internal const string DefaultConfigSectionName = "Neo4j:Client";

    /// <summary>
    /// Gets or sets the Neo4j server <see cref="Uri"/>.
    /// </summary>
    /// <value>
    /// The default value is <see langword="null"/>.
    /// </value>
    public Uri? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the credentials used to authenticate with the Neo4j server.
    /// </summary>
    public IAuthToken? Credentials { get; set; }

    /// <summary>
    /// Parses the provided connection string to extract the Neo4j server endpoint and credentials.
    /// </summary>
    /// <param name="connectionString">The connection string containing the endpoint and optional credentials.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the connection string is null, empty, or invalid.
    /// </exception>
    internal void ParseConnectionString(string? connectionString)
    {
        // Check if the connection string is null or whitespace.
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"""
                    ConnectionString is missing.
                    It should be provided in 'ConnectionStrings:<connectionName>'
                    or '{DefaultConfigSectionName}:Endpoint' key.
                    configuration section.
                    """);
        }

        // Try to create a URI from the connection string.
        if (Uri.TryCreate(connectionString, UriKind.Absolute, out var uri))
        {
            Endpoint = uri;
        }
        else
        {
            // If the connection string is not a valid URI, treat it as a DbConnectionString.
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            // Attempt to extract the 'Endpoint' from the connection string.
            if (builder.TryGetValue("Endpoint", out var endpoint) is false)
            {
                throw new InvalidOperationException($"""
                        The 'ConnectionStrings:<connectionName>' (or 'Endpoint' key in
                        '{DefaultConfigSectionName}') is missing.
                        """);
            }

            // Validate the extracted 'Endpoint' as a URI.
            if (Uri.TryCreate(endpoint.ToString(), UriKind.Absolute, out uri) is false)
            {
                throw new InvalidOperationException($"""
                        The 'ConnectionStrings:<connectionName>' (or 'Endpoint' key in
                        '{DefaultConfigSectionName}') isn't a valid URI.
                        """);
            }

            Endpoint = uri;

            // Attempt to extract 'Username' and 'Password' from the connection string for authentication.
            if (builder.TryGetValue("Username", out var username) &&
               builder.TryGetValue("Password", out var password))
            {
                Credentials = AuthTokens.Basic(username.ToString(), password.ToString());
            }
        }
    }
}
